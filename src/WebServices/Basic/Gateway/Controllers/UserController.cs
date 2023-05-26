using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Aiursoft.DBTools.Models;
using Aiursoft.Gateway.Data;
using Aiursoft.Directory.Models;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Directory.SDK.Models.API.UserAddressModels;
using Aiursoft.Directory.SDK.Models.API.UserViewModels;
using Aiursoft.Gateway.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Identity.Services.Authentication;
using Aiursoft.SDK.Services;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Gateway.Controllers;

[APIExpHandler]
[APIModelStateChecker]
public class UserController : ControllerBase
{
    private readonly IEnumerable<IAuthProvider> _authProviders;
    private readonly CannonService _cannonService;
    private readonly GatewayDbContext _dbContext;
    private readonly GrantChecker _grantChecker;
    private readonly ServiceLocation _serviceLocation;
    private readonly TwoFAHelper _twoFAHelper;
    private readonly UserManager<DirectoryUser> _userManager;

    public UserController(
        UserManager<DirectoryUser> userManager,
        GatewayDbContext context,
        GrantChecker grantChecker,
        TwoFAHelper twoFAHelper,
        IEnumerable<IAuthProvider> authProviders,
        ServiceLocation serviceLocation,
        CannonService cannonService)
    {
        _userManager = userManager;
        _dbContext = context;
        _grantChecker = grantChecker;
        _twoFAHelper = twoFAHelper;
        _authProviders = authProviders;
        _serviceLocation = serviceLocation;
        _cannonService = cannonService;
    }

    [HttpPost]
    public async Task<IActionResult> ChangeProfile(ChangeProfileAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
        user.NickName = model.NewNickName;
        user.IconFilePath = model.NewIconFilePathName;
        user.Bio = model.NewBio;
        await _dbContext.SaveChangesAsync();
        return this.Protocol(new AiurProtocol
            { Code = ErrorType.Success, Message = "Successfully changed this user's profile!" });
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangePassword);
        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return this.Protocol(new AiurProtocol
                { Code = ErrorType.Success, Message = "Successfully changed your password!" });
        }

        return this.Protocol(
            new AiurProtocol { Code = ErrorType.WrongKey, Message = result.Errors.First().Description });
    }

    [Produces(typeof(AiurValue<string>))]
    public async Task<IActionResult> ViewPhoneNumber(ViewPhoneNumberAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ViewPhoneNumber);
        return this.Protocol(new AiurValue<string>(user.PhoneNumber)
        {
            Code = ErrorType.Success,
            Message = "Successfully get the target user's phone number."
        });
    }

    [HttpPost]
    public async Task<IActionResult> SetPhoneNumber(SetPhoneNumberAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangePhoneNumber);
        if (string.IsNullOrWhiteSpace(model.Phone))
        {
            user.PhoneNumber = string.Empty;
        }
        else
        {
            user.PhoneNumber = model.Phone;
        }

        await _userManager.UpdateAsync(user);
        return this.Protocol(ErrorType.Success, "Successfully set the user's PhoneNumber!");
    }

    [Produces(typeof(AiurCollection<UserEmail>))]
    public async Task<IActionResult> ViewAllEmails(ViewAllEmailsAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, null);
        var emails = await _dbContext.UserEmails.Where(t => t.OwnerId == user.Id).ToListAsync();
        return this.Protocol(new AiurCollection<UserEmail>(emails)
        {
            Code = ErrorType.Success,
            Message = "Successfully get the target user's emails."
        });
    }

    [HttpPost]
    public async Task<IActionResult> BindNewEmail(BindNewEmailAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ConfirmEmail);
        var emailExists =
            await _dbContext.UserEmails.AnyAsync(t => t.EmailAddress.ToLower() == model.NewEmail.ToLower());
        if (emailExists)
        {
            return this.Protocol(ErrorType.Conflict, $"An user has already bind email: {model.NewEmail}!");
        }

        var mail = new UserEmail
        {
            OwnerId = user.Id,
            EmailAddress = model.NewEmail.ToLower(),
            Validated = false
        };
        await _dbContext.UserEmails.AddAsync(mail);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(ErrorType.Success, "Successfully set");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmail(DeleteEmailAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ConfirmEmail);

        var userEmails = _dbContext.UserEmails.Where(t => t.OwnerId == user.Id);
        var userEmail =
            await userEmails.SingleOrDefaultAsync(t => t.EmailAddress.ToLower() == model.ThatEmail.ToLower());
        if (userEmail == null)
        {
            return this.Protocol(ErrorType.NotFound, $"Can not find your email:{model.ThatEmail}");
        }

        if (await userEmails.CountAsync() == 1)
        {
            return this.Protocol(ErrorType.Conflict,
                $"Can not delete Email: {model.ThatEmail}, because it was your last Email address!");
        }

        _dbContext.UserEmails.Remove(userEmail);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(ErrorType.Success, $"Successfully deleted the email: {model.ThatEmail}!");
    }

    [HttpPost]
    public async Task<IActionResult> SendConfirmationEmail(SendConfirmationEmailAddressModel model) //User Id
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ConfirmEmail);
        var userEmail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
        if (userEmail == null)
        {
            return this.Protocol(ErrorType.NotFound, $"Can not find your email:{model.Email}");
        }

        if (userEmail.OwnerId != user.Id)
        {
            return this.Protocol(ErrorType.Unauthorized,
                $"The account you tried to authorize is not an account with id: {model.OpenId}");
        }

        if (userEmail.Validated)
        {
            return this.Protocol(ErrorType.HasSuccessAlready, $"The email: {model.Email} was already validated!");
        }

        var byProvider =
            _authProviders.FirstOrDefault(t => user.Email.ToLower().Contains($"@from.{t.GetName().ToLower()}"));
        if (byProvider != null)
        {
            return this.Protocol(ErrorType.UnknownError,
                $"We could not get your email from your auth provider: {byProvider.GetName()} because you set your email private. Please manually link your email at: {_serviceLocation.Account}!");
        }

        // limit the sending frequency to 3 minutes.
        if (DateTime.UtcNow <= userEmail.LastSendTime + new TimeSpan(0, 1, 0))
        {
            return this.Protocol(ErrorType.HasSuccessAlready, "We have just sent you an Email in an minute.");
        }

        var token = Guid.NewGuid().ToString("N");
        userEmail.ValidateToken = token;
        userEmail.LastSendTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        try
        {
            _cannonService.FireAsync<ConfirmationEmailSender>(async sender =>
            {
                await sender.SendConfirmation(user.Id, userEmail.EmailAddress, token);
            });
        }
        catch (SmtpException e)
        {
            return this.Protocol(ErrorType.InvalidInput, e.Message);
        }

        return this.Protocol(ErrorType.Success, "Successfully sent the validation email.");
    }

    [HttpPost]
    public async Task<IActionResult> SetPrimaryEmail(SetPrimaryEmailAddressModel model) //User Id
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ConfirmEmail);
        var userEmail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
        if (userEmail == null)
        {
            return this.Protocol(ErrorType.NotFound, $"Can not find your email:{model.Email}");
        }

        if (userEmail.OwnerId != user.Id)
        {
            return this.Protocol(ErrorType.Unauthorized,
                $"The account you tried to authorize is not an account with id: {model.OpenId}");
        }

        if (!userEmail.Validated)
        {
            return this.Protocol(ErrorType.InsufficientPermissions, $"The email :{model.Email} was not validated!");
        }

        userEmail.Priority = user.Emails.Max(t => t.Priority) + 1;
        await _dbContext.SaveChangesAsync();
        return this.Protocol(ErrorType.Success, "Successfully set your primary email.");
    }

    [Produces(typeof(AiurCollection<AppGrant>))]
    public async Task<IActionResult> ViewGrantedApps(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeGrantInfo);
        var applications = await _dbContext.LocalAppGrant.Where(t => t.GatewayUserId == user.Id).ToListAsync();
        return this.Protocol(new AiurCollection<AppGrant>(applications)
        {
            Code = ErrorType.Success,
            Message = "Successfully get all your granted apps!"
        });
    }

    [HttpPost]
    public async Task<IActionResult> DropGrantedApps(DropGrantedAppsAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeGrantInfo);
        var appToDelete = await _dbContext
            .LocalAppGrant
            .Where(t => t.GatewayUserId == user.Id)
            .SingleOrDefaultAsync(t => t.AppId == model.AppIdToDrop);
        if (appToDelete == null)
        {
            return this.Protocol(ErrorType.NotFound,
                $"Can not find target grant record with app with id: {model.AppIdToDrop}");
        }

        _dbContext.LocalAppGrant.Remove(appToDelete);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(ErrorType.Success, "Successfully deleted target app grant record!");
    }

    [Produces(typeof(AiurPagedCollection<AuditLog>))]
    public async Task<IActionResult> ViewAuditLog(ViewAuditLogAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ViewAuditLog);
        var query = _dbContext
            .AuditLogs
            .Where(t => t.UserId == user.Id)
            .OrderByDescending(t => t.HappenTime);

        return this.Protocol(await AiurPagedCollectionBuilder.BuildAsync<AuditLog>(
            query,
            model,
            ErrorType.Success,
            "Successfully get all your audit log!"));
    }

    [Produces(typeof(AiurCollection<AiurThirdPartyAccount>))]
    public async Task<IActionResult> ViewSocialAccounts(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ManageSocialAccount);
        var accounts = await _dbContext
            .ThirdPartyAccounts
            .Where(t => t.OwnerId == user.Id)
            .OrderByDescending(t => t.BindTime)
            .ToListAsync();
        return this.Protocol(new AiurCollection<ThirdPartyAccount>(accounts)
        {
            Code = ErrorType.Success,
            Message = "Successfully get all your audit log!"
        });
    }

    [HttpPost]
    public async Task<IActionResult> UnBindSocialAccount(UnBindSocialAccountAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ManageSocialAccount);
        var accounts = await _dbContext
            .ThirdPartyAccounts
            .Where(t => t.OwnerId == user.Id)
            .Where(t => t.ProviderName.ToLower() == model.ProviderName.ToLower())
            .ToListAsync();
        _dbContext.ThirdPartyAccounts.RemoveRange(accounts);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(ErrorType.Success, $"Successfully unbound your {model.ProviderName} account.");
    }

    [HttpPost]
    [Produces(typeof(AiurValue<bool>))]
    public async Task<IActionResult> ViewHas2FAKey(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
        var key = user.Has2FAKey;
        return this.Protocol(new AiurValue<bool>(key)
        {
            Code = ErrorType.Success,
            Message = "Successfully get the target user's Has2FAkey."
        });
    }

    [HttpPost]
    [Produces(typeof(AiurValue<bool>))]
    public async Task<IActionResult> ViewTwoFactorEnabled(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
        var enabled = user.TwoFactorEnabled;
        return this.Protocol(new AiurValue<bool>(enabled)
        {
            Code = ErrorType.Success,
            Message = "Successfully get the target user's TwoFactorEnabled."
        });
    }

    [HttpPost]
    [Produces(typeof(View2FAKeyViewModel))]
    public async Task<IActionResult> View2FAKey(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
        var (twoFAKey, twoFAQRUri) = await _twoFAHelper.LoadSharedKeyAndQrCodeUriAsync(user);
        return this.Protocol(new View2FAKeyViewModel
        {
            TwoFAKey = twoFAKey,
            TwoFAQRUri = twoFAQRUri,
            Code = ErrorType.Success,
            Message = "Successfully set the user's TwoFAKey!"
        });
    }

    [HttpPost]
    [Produces(typeof(AiurValue<bool>))]
    public async Task<IActionResult> SetTwoFAKey(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
        if (!user.Has2FAKey)
        {
            user.Has2FAKey = true;
            await _userManager.UpdateAsync(user);
        }

        var hasKey = user.Has2FAKey;
        return this.Protocol(new AiurValue<bool>(hasKey)
        {
            Code = ErrorType.Success,
            Message = "Successfully set the user's TwoFAKey!"
        });
    }

    [HttpPost]
    public async Task<IActionResult> ResetTwoFAKey(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);

        // reset 2fa key
        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        return this.Protocol(ErrorType.Success, "Successfully reset the user's TwoFAKey!");
    }

    [HttpPost]
    [Produces(typeof(AiurValue<bool>))]
    public async Task<IActionResult> TwoFAVerifyCode(TwoFAVerifyCodeAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);

        // Strip spaces and hypens
        var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2FATokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2FATokenValid || user.TwoFactorEnabled)
        {
            return this.Protocol(new AiurValue<bool>(is2FATokenValid)
            {
                Code = ErrorType.Success,
                Message = "Sucess Verified code."
            });
        }

        // enable 2fa.
        user.TwoFactorEnabled = true;
        await _userManager.UpdateAsync(user);

        return this.Protocol(new AiurValue<bool>(true)
        {
            Code = ErrorType.Success,
            Message = "Successfully Verified code."
        });
    }

    [HttpPost]
    [Produces(typeof(AiurValue<bool>))]
    public async Task<IActionResult> DisableTwoFA(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);

        var disable2FAResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (disable2FAResult.Succeeded)
        {
            user.TwoFactorEnabled = false;
            user.Has2FAKey = false;
            await _userManager.ResetAuthenticatorKeyAsync(user);
            await _userManager.UpdateAsync(user);
        }

        var success = disable2FAResult.Succeeded;

        return this.Protocol(new AiurValue<bool>(success)
        {
            Code = ErrorType.Success,
            Message = "Successfully called DisableTwoFA method!"
        });
    }

    [HttpPost]
    [Produces(typeof(AiurCollection<string>))]
    public async Task<IActionResult> GetRecoveryCodes(UserOperationAddressModel model)
    {
        var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        return this.Protocol(new AiurCollection<string>(recoveryCodes.ToList())
        {
            Code = ErrorType.Success,
            Message = "Sucess regenerate recovery Codes!."
        });
    }
}
using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Gateway.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models.API.UserAddressModels;
using Aiursoft.Pylon.Models.API.UserViewModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class UserController : Controller
    {
        private readonly UserManager<GatewayUser> _userManager;
        private readonly GatewayDbContext _dbContext;
        private readonly ConfirmationEmailSender _emailSender;
        private readonly GrantChecker _grantChecker;
        private readonly TwoFAHelper _twoFAHelper;
        private readonly IEnumerable<IAuthProvider> _authProviders;
        private readonly ServiceLocation _serviceLocation;

        public UserController(
            UserManager<GatewayUser> userManager,
            GatewayDbContext context,
            ConfirmationEmailSender emailSender,
            GrantChecker grantChecker,
            TwoFAHelper twoFAHelper,
            IEnumerable<IAuthProvider> authProviders,
            ServiceLocation serviceLocation)
        {
            _userManager = userManager;
            _dbContext = context;
            _emailSender = emailSender;
            _grantChecker = grantChecker;
            _twoFAHelper = twoFAHelper;
            _authProviders = authProviders;
            _serviceLocation = serviceLocation;
        }

        [HttpPost]
        public async Task<JsonResult> ChangeProfile(ChangeProfileAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            user.NickName = model.NewNickName;
            user.IconFilePath = model.NewIconFilePathName;
            user.Bio = model.NewBio;
            await _dbContext.SaveChangesAsync();
            return Json(new AiurProtocol { Code = ErrorType.Success, Message = "Successfully changed this user's profile!" });
        }

        [HttpPost]
        public async Task<JsonResult> ChangePassword(ChangePasswordAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangePassword);
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Json(new AiurProtocol { Code = ErrorType.Success, Message = "Successfully changed your password!" });
            }
            else
            {
                return Json(new AiurProtocol { Code = ErrorType.WrongKey, Message = result.Errors.First().Description });
            }
        }

        [APIProduces(typeof(AiurValue<string>))]
        public async Task<IActionResult> ViewPhoneNumber(ViewPhoneNumberAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ViewPhoneNumber);
            return Json(new AiurValue<string>(user.PhoneNumber)
            {
                Code = ErrorType.Success,
                Message = "Successfully get the target user's phone number."
            });
        }

        [HttpPost]
        public async Task<JsonResult> SetPhoneNumber(SetPhoneNumberAddressModel model)
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

        [APIProduces(typeof(AiurCollection<UserEmail>))]
        public async Task<IActionResult> ViewAllEmails(ViewAllEmailsAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, null);
            var emails = await _dbContext.UserEmails.Where(t => t.OwnerId == user.Id).ToListAsync();
            return Json(new AiurCollection<UserEmail>(emails)
            {
                Code = ErrorType.Success,
                Message = "Successfully get the target user's emails."
            });
        }

        [HttpPost]
        public async Task<IActionResult> BindNewEmail(BindNewEmailAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ConfirmEmail);
            var emailexists = await _dbContext.UserEmails.AnyAsync(t => t.EmailAddress.ToLower() == model.NewEmail.ToLower());
            if (emailexists)
            {
                return this.Protocol(ErrorType.NotEnoughResources, $"An user has already bind email: {model.NewEmail}!");
            }
            var mail = new UserEmail
            {
                OwnerId = user.Id,
                EmailAddress = model.NewEmail.ToLower(),
                Validated = false
            };
            _dbContext.UserEmails.Add(mail);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully set");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmail(DeleteEmailAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ConfirmEmail);

            var userEmails = _dbContext.UserEmails.Where(t => t.OwnerId == user.Id);
            var useremail = await userEmails.SingleOrDefaultAsync(t => t.EmailAddress.ToLower() == model.ThatEmail.ToLower());
            if (useremail == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Can not find your email:{model.ThatEmail}");
            }
            if (await userEmails.CountAsync() == 1)
            {
                return this.Protocol(ErrorType.NotEnoughResources, $"Can not delete Email: {model.ThatEmail}, because it was your last Email address!");
            }
            _dbContext.UserEmails.Remove(useremail);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"Successfully deleted the email: {model.ThatEmail}!");
        }

        [HttpPost]
        public async Task<IActionResult> SendConfirmationEmail(SendConfirmationEmailAddressModel model)//User Id
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ConfirmEmail);
            var useremail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
            if (useremail == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Can not find your email:{model.Email}");
            }
            if (useremail.OwnerId != user.Id)
            {
                return this.Protocol(ErrorType.Unauthorized, $"The account you tried to authorize is not an account with id: {model.OpenId}");
            }
            if (useremail.Validated)
            {
                return this.Protocol(ErrorType.HasDoneAlready, $"The email: {model.Email} was already validated!");
            }
            var byProvider = _authProviders.FirstOrDefault(t => user.Email.ToLower().Contains($"@from.{t.GetName().ToLower()}"));
            if (byProvider != null)
            {
                return this.Protocol(ErrorType.HasDoneAlready, $"We could not get your email from your auth provider: {byProvider.GetName()} because you set your email private. Please manually link your email at: {_serviceLocation.Account}!");
            }
            // limit the sending frenquency to 3 minutes.
            if (DateTime.UtcNow > useremail.LastSendTime + new TimeSpan(0, 1, 0))
            {
                var token = Guid.NewGuid().ToString("N");
                useremail.ValidateToken = token;
                useremail.LastSendTime = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                try
                {
                    await _emailSender.SendConfirmation(user.Id, useremail.EmailAddress, token);
                }
                catch (SmtpException e)
                {
                    return this.Protocol(ErrorType.InvalidInput, e.Message);
                }
                return this.Protocol(ErrorType.Success, "Successfully sent the validation email.");
            }
            return this.Protocol(ErrorType.RequireAttention, "We have just sent you an Email in an minute.");
        }

        [HttpPost]
        public async Task<IActionResult> SetPrimaryEmail(SetPrimaryEmailAddressModel model)//User Id
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ConfirmEmail);
            var userEmail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
            if (userEmail == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Can not find your email:{model.Email}");
            }
            if (userEmail.OwnerId != user.Id)
            {
                return this.Protocol(ErrorType.Unauthorized, $"The account you tried to authorize is not an account with id: {model.OpenId}");
            }
            if (!userEmail.Validated)
            {
                return this.Protocol(ErrorType.Pending, $"The email :{model.Email} was not validated!");
            }
            userEmail.Priority = user.Emails.Max(t => t.Priority) + 1;
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully set your primary email.");
        }

        [APIProduces(typeof(AiurCollection<AppGrant>))]
        public async Task<IActionResult> ViewGrantedApps(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeGrantInfo);
            var applications = await _dbContext.LocalAppGrant.Where(t => t.GatewayUserId == user.Id).ToListAsync();
            return Json(new AiurCollection<AppGrant>(applications)
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
                .SingleOrDefaultAsync(t => t.AppID == model.AppIdToDrop);
            if (appToDelete == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Can not find target grant record with app with id: {model.AppIdToDrop}");
            }
            _dbContext.LocalAppGrant.Remove(appToDelete);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully deleted target app grant record!");
        }

        [APIProduces(typeof(AiurPagedCollection<AuditLog>))]
        public async Task<IActionResult> ViewAuditLog(ViewAuditLogAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ViewAuditLog);
            var logsQuery = _dbContext
                .AuditLogs
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.HappenTime);
            var logs = await logsQuery
                .Skip(model.PageNumber * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            var logsCount = await logsQuery
                .CountAsync();
            return Json(new AiurPagedCollection<AuditLogLocal>(logs)
            {
                Code = ErrorType.Success,
                Message = "Successfully get all your audit log!",
                TotalCount = logsCount
            });
        }

        [APIProduces(typeof(AiurCollection<AiurThirdPartyAccount>))]
        public async Task<IActionResult> ViewSocialAccounts(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ManageSocialAccount);
            var accounts = await _dbContext
                .ThirdPartyAccounts
                .Where(t => t.OwnerId == user.Id)
                .OrderByDescending(t => t.BindTime)
                .ToListAsync();
            return Json(new AiurCollection<ThirdPartyAccount>(accounts)
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
        [APIProduces(typeof(AiurValue<bool>))]
        public async Task<JsonResult> ViewHas2FAkey(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            bool key = user.Has2FAKey;
            return Json(new AiurValue<bool>(key)
            {
                Code = ErrorType.Success,
                Message = "Successfully get the target user's Has2FAkey."
            });
        }

        [HttpPost]
        [APIProduces(typeof(AiurValue<bool>))]
        public async Task<JsonResult> ViewTwoFactorEnabled(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            bool enabled = user.TwoFactorEnabled;
            return Json(new AiurValue<bool>(enabled)
            {
                Code = ErrorType.Success,
                Message = "Successfully get the target user's TwoFactorEnabled."
            });
        }

        [HttpPost]
        [APIProduces(typeof(View2FAKeyViewModel))]
        public async Task<IActionResult> View2FAKey(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            var (twoFAKey, twoFAQRUri) = await _twoFAHelper.LoadSharedKeyAndQrCodeUriAsync(user);
            return Json(new View2FAKeyViewModel
            {
                TwoFAKey = twoFAKey,
                TwoFAQRUri = twoFAQRUri,
                Code = ErrorType.Success,
                Message = "Successfully set the user's TwoFAKey!"
            });
        }

        [HttpPost]
        [APIProduces(typeof(AiurValue<bool>))]
        public async Task<IActionResult> SetTwoFAKey(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            if (!user.Has2FAKey)
            {
                user.Has2FAKey = true;
                await _userManager.UpdateAsync(user);
            }
            var hasKey = user.Has2FAKey;
            return Json(new AiurValue<bool>(hasKey)
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
        [APIProduces(typeof(AiurValue<bool>))]
        public async Task<IActionResult> TwoFAVerificyCode(TwoFAVerificyCodeAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (is2faTokenValid)
            {
                // enable 2fa.
                if (!user.TwoFactorEnabled)
                {
                    user.TwoFactorEnabled = true;
                    await _userManager.UpdateAsync(user);
                }
            }

            return Json(new AiurValue<bool>(is2faTokenValid)
            {
                Code = ErrorType.Success,
                Message = "Sucess Verified code."
            });
        }

        [HttpPost]
        [APIProduces(typeof(AiurValue<bool>))]
        public async Task<IActionResult> DisableTwoFA(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (true == disable2faResult.Succeeded)
            {
                user.TwoFactorEnabled = false;
                user.Has2FAKey = false;
                await _userManager.ResetAuthenticatorKeyAsync(user);
                await _userManager.UpdateAsync(user);
            }
            bool success = disable2faResult.Succeeded;

            return Json(new AiurValue<bool>(success)
            {
                Code = ErrorType.Success,
                Message = "Successfully called DisableTwoFA method!"
            });
        }

        [HttpPost]
        [APIProduces(typeof(AiurCollection<string>))]
        public async Task<IActionResult> GetRecoveryCodes(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            return Json(new AiurCollection<string>(recoveryCodes.ToList())
            {
                Code = ErrorType.Success,
                Message = "Sucess regenerate recovery Codes!."
            });
        }
    }
}
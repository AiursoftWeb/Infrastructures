using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Gateway.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.UserAddressModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
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
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public UserController(
             UserManager<GatewayUser> userManager,
             GatewayDbContext context,
             ConfirmationEmailSender emailSender,
             GrantChecker grantChecker,
              UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _dbContext = context;
            _emailSender = emailSender;
            _grantChecker = grantChecker;
            _urlEncoder = urlEncoder;
        }

        public async Task<JsonResult> ChangeProfile(ChangeProfileAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            user.NickName = model.NewNickName;
            user.IconFilePath = model.NewIconFilePathName;
            user.Bio = model.NewBio;
            await _dbContext.SaveChangesAsync();
            return Json(new AiurProtocol { Code = ErrorType.Success, Message = "Successfully changed this user's profile!" });
        }

        public async Task<JsonResult> ChangePassword(ChangePasswordAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangePassword);
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Json(new AiurProtocol { Code = ErrorType.Success, Message = "Successfully " });
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
            if (userEmail.Priority == user.Emails.Max(t => t.Priority))
            {
                return this.Protocol(ErrorType.HasDoneAlready, $"The email :{model.Email} was already primary email!");
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

        [APIProduces(typeof(AiurCollection<AuditLogLocal>))]
        public async Task<IActionResult> ViewAuditLog(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ViewAuditLog);
            var logs = await _dbContext
                .AuditLogs
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.HappenTime)
                .ToListAsync();
            return Json(new AiurCollection<AuditLogLocal>(logs)
            {
                Code = ErrorType.Success,
                Message = "Successfully get all your audit log!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> ViewTwoFAKey(SetTwoFAAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            var returnList = new List<SetTwoFAAddressModel>();
            returnList = await TwoFAAuthority(user, model, returnList);
            return Json(new AiurCollection<SetTwoFAAddressModel>(returnList)
            {
                Code = ErrorType.Success,
                Message = "Successfully set the user's TwoFAKey!"
            });
        }

        [HttpPost]
        [APIProduces(typeof(AiurCollection<SetTwoFAAddressModel>))]
        public async Task<IActionResult> SetTwoFAKey(SetTwoFAAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            var returnList = new List<SetTwoFAAddressModel>();
            returnList = await LoadSharedKeyAndQrCodeUriAsync(user, model);
            if (!user.HasAuthenticator)
            {
                user.HasAuthenticator = true;
                user.HasAuthenticator = model.HasAuthenticator;
                await _userManager.UpdateAsync(user);
            }
            return Json(new AiurCollection<SetTwoFAAddressModel>(returnList)
            {
                Code = ErrorType.Success,
                Message = "Successfully set the user's TwoFAKey!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetTwoFAKey(SetTwoFAAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);

            var returnList = await LoadSharedKeyAndQrCodeUriAsync(user, model);
            model.HasAuthenticator = user.HasAuthenticator;
            return Json(new AiurCollection<SetTwoFAAddressModel>(returnList)
            {
                Code = ErrorType.Success,
                Message = "Successfully set the user's TwoFAKey!"
            });
        }

        [HttpPost]
        public async Task<JsonResult> TwoFAVerificyCode(SetTwoFAAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);

                return Json(new AiurValue<string>(null)
                {
                    Code = ErrorType.Success,
                    Message = "Verification code is invalid."
                });
            }
            else
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                var recodeArray = recoveryCodes.ToArray();
                //var len = recodeArray.Length;
                model.RecoveryCodesKey = null;
                foreach (var i in recodeArray)
                {
                    model.RecoveryCodesKey += i;
                }
                return Json(new AiurValue<string>(model.RecoveryCodesKey)
                {
                    Code = ErrorType.Success,
                    Message = "Sucess Verified code ."
                });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> DisableTwoFA(DisableTwoFAAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);

            string returnValue = null;
            if (user == null)
            {
                returnValue = "Userisnull";
            }
            else
            {
                var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                if (!disable2faResult.Succeeded)
                {
                    returnValue = "error";
                }
                else
                {                   
                    user.HasAuthenticator = false;
                    await _dbContext.SaveChangesAsync();
                    returnValue = "succeeded";
                }
            }

            return Json(new AiurValue<string>(returnValue)
            {
                Code = ErrorType.Success,
                Message = "Successfully called DisableTwoFA method!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> RegenerateRecoveryCodes(RegenerateRecoveryCodesAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeBasicInfo);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var recodeArray = recoveryCodes.ToArray();

            model.RecoveryCodesKey = null;
            foreach (var i in recodeArray)
            {
                model.RecoveryCodesKey += i;
            }
            return Json(new AiurValue<string>(model.RecoveryCodesKey)
            {
                Code = ErrorType.Success,
                Message = "Sucess regenerate recovery Codes!."
            });
        }

        #region --- TwoFAKey Helper---

        private async Task<List<SetTwoFAAddressModel>> LoadSharedKeyAndQrCodeUriAsync(GatewayUser user, SetTwoFAAddressModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            model.TwoFAKey = FormatKey(unformattedKey);
            model.Is2faEnabled = user.TwoFactorEnabled;
            model.HasAuthenticator = unformattedKey != null;

            var ReturnList = new List<SetTwoFAAddressModel>();
            ReturnList.Add(model);
            return ReturnList;

            //model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }
        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        public async Task<List<SetTwoFAAddressModel>> TwoFAAuthority(GatewayUser user, SetTwoFAAddressModel model, List<SetTwoFAAddressModel> returnlist)
        {
            if (user.HasAuthenticator)
            {
                returnlist = await LoadSharedKeyAndQrCodeUriAsync(user, model);
            }
            else
            {
                model.HasAuthenticator = user.HasAuthenticator;
                model.Is2faEnabled = user.TwoFactorEnabled;
                model.TwoFAKey = "";
                returnlist.Add(model);
            }
            return returnlist;
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Account.TwoFAKey"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion
    }
}
using Aiursoft.API.Data;
using Aiursoft.API.Models;
using Aiursoft.API.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.UserAddressModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Aiursoft.API.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class UserController : Controller
    {
        private readonly UserManager<APIUser> _userManager;
        private readonly APIDbContext _dbContext;
        private readonly ConfirmationEmailSender _emailSender;
        private readonly GrantChecker _grantChecker;

        public UserController(
            UserManager<APIUser> userManager,
            APIDbContext context,
            ConfirmationEmailSender emailSender,
            GrantChecker grantChecker)
        {
            _userManager = userManager;
            _dbContext = context;
            _emailSender = emailSender;
            _grantChecker = grantChecker;
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
        public async Task<IActionResult> ViewGrantedApps(ViewGrantedAppsAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ChangeGrantInfo);
            var applications = await _dbContext.LocalAppGrant.Where(t => t.APIUserId == user.Id).ToListAsync();
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
                .Where(t => t.APIUserId == user.Id)
                .SingleOrDefaultAsync(t => t.AppID == model.AppIdToDrop);
            if (appToDelete == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Can not find target grant record with app with id: {model.AppIdToDrop}");
            }
            _dbContext.LocalAppGrant.Remove(appToDelete);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully deleted target app grant record!");
        }

        [APIProduces(typeof(AiurCollection<AuditLog>))]
        public async Task<IActionResult> ViewAuditLog(UserOperationAddressModel model)
        {
            var user = await _grantChecker.EnsureGranted(model.AccessToken, model.OpenId, t => t.ViewAuditLog);
            var logs = await _dbContext.AuditLogs.Where(t => t.UserId == user.Id).ToListAsync();
            return Json(new AiurCollection<AuditLog>(logs)
            {
                Code = ErrorType.Success,
                Message = "Successfully get all your audit log!"
            });
        }
    }
}
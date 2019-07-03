using Aiursoft.API.Data;
using Aiursoft.API.Models;
using Aiursoft.API.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.AccountAddressModels;
using Aiursoft.Pylon.Models.API.AccountViewModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToDeveloperServer;
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
    public class AccountController : Controller
    {
        private readonly ACTokenManager _tokenManager;
        private readonly DeveloperApiService _apiService;
        private readonly APIDbContext _dbContext;
        private readonly SignInManager<APIUser> _signInManager;
        private readonly UserManager<APIUser> _userManager;
        private readonly ConfirmationEmailSender _emailSender;

        public AccountController(
            ACTokenManager tokenManager,
            DeveloperApiService apiService,
            APIDbContext dbContext,
            SignInManager<APIUser> signInManager,
            UserManager<APIUser> userManager,
            ConfirmationEmailSender emailSender)
        {
            _tokenManager = tokenManager;
            _apiService = apiService;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        [APIProduces(typeof(AiurValue<int>))]
        public async Task<IActionResult> PasswordAuth(PasswordAuthAddressModel model)
        {
            var appId = _tokenManager.ValidateAccessToken(model.AccessToken);
            var mail = await _dbContext
                .UserEmails
                .Include(t => t.Owner)
                .SingleOrDefaultAsync(t => t.EmailAddress == model.Email);
            if (mail == null)
            {
                return this.Protocol(ErrorType.NotFound, $"The account with email {model.Email} was not found!");
            }
            var user = mail.Owner;
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                if (!await user.HasAuthorizedApp(_dbContext, appId))
                {
                    await user.GrantTargetApp(_dbContext, appId);
                }
                var pack = await user.GeneratePack(_dbContext, appId);
                return Json(new AiurValue<int>(pack.Code)
                {
                    Code = ErrorType.Success,
                    Message = "Auth success."
                });
            }
            else if (result.RequiresTwoFactor)
            {
                throw new NotImplementedException();
            }
            else if (result.IsLockedOut)
            {
                return this.Protocol(ErrorType.Unauthorized, $"The account with email {model.Email} was locked! Please try again several minutes later!");
            }
            else
            {
                return this.Protocol(ErrorType.Unauthorized, "Wrong password!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AppRegister(AppRegisterAddressModel model)
        {
            var appId = _tokenManager.ValidateAccessToken(model.AccessToken);
            bool exists = _dbContext.UserEmails.Any(t => t.EmailAddress == model.Email.ToLower());
            if (exists)
            {
                return this.Protocol(ErrorType.NotEnoughResources, $"A user with email '{model.Email}' already exists!");
            }
            var user = new APIUser
            {
                UserName = model.Email,
                Email = model.Email,
                NickName = model.Email.Split('@')[0],
                PreferedLanguage = "en",
                HeadImgFileKey = Values.DefaultImageId
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var primaryMail = new UserEmail
                {
                    EmailAddress = model.Email.ToLower(),
                    OwnerId = user.Id,
                    ValidateToken = Guid.NewGuid().ToString("N")
                };
                _dbContext.UserEmails.Add(primaryMail);
                await _dbContext.SaveChangesAsync();
                // Send him an confirmation email here:
                try
                {
                    await _emailSender.SendConfirmation(user.Id, primaryMail.EmailAddress, primaryMail.ValidateToken);
                }
                // Ignore smtp exception.
                catch (SmtpException) { }
                // Grant this app.
                if (!await user.HasAuthorizedApp(_dbContext, appId))
                {
                    await user.GrantTargetApp(_dbContext, appId);
                }
                return this.Protocol(ErrorType.Success, "Successfully created your account.");
            }
            return this.Protocol(ErrorType.NotEnoughResources, result.Errors.First().Description);
        }

        [APIProduces(typeof(CodeToOpenIdViewModel))]
        public async Task<IActionResult> CodeToOpenId(CodeToOpenIdAddressModel model)
        {
            var appId = _tokenManager.ValidateAccessToken(model.AccessToken);
            var targetPack = await _dbContext
                .OAuthPack
                .SingleOrDefaultAsync(t => t.Code == model.Code);

            if (targetPack == null)
            {
                return this.Protocol(ErrorType.WrongKey, "The code doesn't exists in our database.");
            }
            // Use time is more than 10 seconds from now.
            if (targetPack.UseTime != DateTime.MinValue && targetPack.UseTime + TimeSpan.FromSeconds(10) < DateTime.UtcNow)
            {
                return this.Protocol(ErrorType.HasDoneAlready, "Code is used already!");
            }
            if (targetPack.ApplyAppId != appId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The app granted code is not the app granting access token!");
            }
            var capp = (await _apiService.AppInfoAsync(targetPack.ApplyAppId)).App;
            if (!capp.ViewOpenId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The app doesn't have view open id permission.");
            }
            targetPack.UseTime = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            var viewModel = new CodeToOpenIdViewModel
            {
                OpenId = targetPack.UserId,
                Scope = "scope",
                Message = "Successfully get user openid",
                Code = ErrorType.Success
            };
            return Json(viewModel);
        }

        [APIProduces(typeof(UserInfoViewModel))]
        public async Task<IActionResult> UserInfo(UserInfoAddressModel model)
        {
            var appId = _tokenManager.ValidateAccessToken(model.AccessToken);
            var user = await _dbContext.Users.Include(t => t.Emails).SingleOrDefaultAsync(t => t.Id == model.OpenId);
            if (user == null)
            {
                return this.Protocol(ErrorType.NotFound, "Can not find a user with open id: " + model.OpenId);
            }
            if (!await user.HasAuthorizedApp(_dbContext, appId))
            {
                return this.Protocol(ErrorType.NotFound, "The user did not allow your app to view his personal info! App Id: " + model.OpenId);
            }
            var viewModel = new UserInfoViewModel
            {
                Code = 0,
                Message = "Successfully get target user info.",
                User = user
            };
            return Json(viewModel);
        }
    }
}

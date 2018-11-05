using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.API.Services;
using Aiursoft.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Aiursoft.API.Models.OAuthViewModels;
using Aiursoft.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Models;
using System.Linq;
using Microsoft.Extensions.Localization;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Aiursoft.Pylon.Models.API.OAuthViewModels;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models.Developer;

namespace Aiursoft.API.Controllers
{
    public class OAuthController : Controller
    {
        //Ensure App
        //  if app is null
        //  return not found
        //Ensure model state
        //  if model state invalid in HTTP Get
        //      return autherror
        //  if model state invalid in HTTP Post
        //      return view with model
        //Prepare user
        //Check validation
        //Do jobs
        //Return success message

        private readonly UserManager<APIUser> _userManager;
        private readonly SignInManager<APIUser> _signInManager;
        private readonly ILogger _logger;
        private readonly APIDbContext _dbContext;
        private readonly IStringLocalizer<OAuthController> _localizer;
        private readonly ServiceLocation _serviceLocation;
        private readonly DeveloperApiService _apiService;

        public OAuthController(
            UserManager<APIUser> userManager,
            SignInManager<APIUser> signInManager,
            ILoggerFactory loggerFactory,
            APIDbContext _context,
            IStringLocalizer<OAuthController> localizer,
            ServiceLocation serviceLocation,
            DeveloperApiService developerApiService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<OAuthController>();
            _dbContext = _context;
            _localizer = localizer;
            _serviceLocation = serviceLocation;
            _apiService = developerApiService;
        }

        //http://localhost:53657/oauth/authorize?appid=29bf5250a6d93d47b6164ac2821d5009&redirect_uri=http%3A%2F%2Flocalhost%3A55771%2FAuth%2FAuthResult&response_type=code&scope=snsapi_base&state=http%3A%2F%2Flocalhost%3A55771%2FAuth%2FGoAuth#aiursoft_redirect
        [HttpGet]
        public async Task<IActionResult> Authorize(AuthorizeAddressModel model)
        {
            var app = (await _apiService.AppInfoAsync(model.appid)).App;
            if (app == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var url = new Uri(model.redirect_uri);
            var user = await GetCurrentUserAsync();
            // Wrong domain
            if (url.Host != app.AppDomain && app.DebugMode == false)
            {
                ModelState.AddModelError(string.Empty, "Redirect uri did not work in the valid domain!");
                _logger.LogInformation($"A request with appId {model.appid} is access wrong domian.");
                return View("AuthError");
            }
            // Signed in but have to input info.
            else if (user != null && app.ForceInputPassword == false && model.forceConfirm != true)
            {
                return await FinishAuth(model.Convert(user.Email), app.ForceConfirmation);
            }
            // Not signed in but we don't want his info
            else if (model.tryAutho == true)
            {
                return Redirect($"{url.Scheme}://{url.Host}:{url.Port}/?{Values.DirectShowString.Key}={Values.DirectShowString.Value}");
            }
            var viewModel = new AuthorizeViewModel(model.redirect_uri, model.state, model.appid, model.scope, model.response_type, app.AppName, app.AppIconAddress);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Authorize(AuthorizeViewModel model)
        {
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            if (app == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var mail = await _dbContext
                .UserEmails
                .Include(t => t.Owner)
                .SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
            if (mail == null)
            {
                ModelState.AddModelError(string.Empty, "Unknown user email.");
                model.Recover(app.AppName, app.AppIconAddress);
                return View(model);
            }
            var user = mail.Owner;
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return await FinishAuth(model, app.ForceConfirmation);
            }
            else if (result.RequiresTwoFactor)
            {
                throw new NotImplementedException();
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "The account is locked for too many attempts.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The password does not match our records.");
            }
            model.Recover(app.AppName, app.AppIconAddress);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AuthorizeConfirm(AuthorizeConfirmAddressModel model)
        {
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            if (app == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var user = await GetCurrentUserAsync();
            var viewModel = new AuthorizeConfirmViewModel
            {
                AppName = app.AppName,
                UserNickName = user.NickName,
                AppId = model.AppId,
                ToRedirect = model.ToRedirect,
                State = model.State,
                Scope = model.Scope,
                ResponseType = model.ResponseType,
                UserIconId = user.HeadImgFileKey,
                // Permissions
                ViewOpenId = app.ViewOpenId,
                ViewPhoneNumber = app.ViewPhoneNumber,
                ChangePhoneNumber = app.ChangePhoneNumber,
                ConfirmEmail = app.ConfirmEmail,
                ChangeBasicInfo = app.ChangeBasicInfo,
                ChangePassword = app.ChangePassword,
                TermsUrl = app.LicenseUrl,
                PStatementUrl = app.PrivacyStatementUrl
            };
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AuthorizeConfirm(AuthorizeConfirmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            model.Email = user.Email;
            await user.GrantTargetApp(_dbContext, model.AppId);
            return await FinishAuth(model);
        }

        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> PasswordAuth(PasswordAuthAddressModel model)
        {
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            if (app == null)
            {
                return NotFound();
            }
            var mail = await _dbContext
                .UserEmails
                .Include(t => t.Owner)
                .SingleOrDefaultAsync(t => t.EmailAddress == model.Email);
            if (mail == null)
            {
                return this.Protocal(ErrorType.NotFound, $"The account with email {model.Email} was not found!");
            }
            var user = mail.Owner;
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                if (!await user.HasAuthorizedApp(_dbContext, app.AppId))
                {
                    await user.GrantTargetApp(_dbContext, app.AppId);
                }
                var pack = await user.GeneratePack(_dbContext, app.AppId);
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
                return this.Protocal(ErrorType.InvalidInput, $"The account with email {model.Email} was locked! Please try again several minutes later!");
            }
            else
            {
                return this.Protocal(ErrorType.Unauthorized, "Wrong password!");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register(AuthorizeAddressModel model)
        {
            var app = (await _apiService.AppInfoAsync(model.appid)).App;
            if (app == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var viewModel = new RegisterViewModel(model.redirect_uri, model.state, model.appid, model.scope, model.response_type, app.AppName, app.AppIconAddress);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var capp = (await _apiService.AppInfoAsync(model.AppId)).App;
            if (capp == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                model.Recover(capp.AppName, capp.AppIconAddress);
                return View(model);
            }
            bool exists = _dbContext.UserEmails.Any(t => t.EmailAddress == model.Email.ToLower());
            if (exists)
            {
                ModelState.AddModelError(string.Empty, $"A user with email '{model.Email}' already exists!");
                model.Recover(capp.AppName, capp.AppIconAddress);
                return View(model);
            }
            var user = new APIUser
            {
                UserName = model.Email,
                Email = model.Email,
                NickName = model.Email.Split('@')[0],
                PreferedLanguage = model.PreferedLanguage,
                HeadImgFileKey = Values.DefaultImageId
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var primaryMail = new UserEmail
                {
                    EmailAddress = model.Email.ToLower(),
                    OwnerId = user.Id
                };
                _dbContext.UserEmails.Add(primaryMail);
                await _dbContext.SaveChangesAsync();
                await _signInManager.SignInAsync(user, isPersistent: true);
                return await FinishAuth(model);
            }
            AddErrors(result);
            model.Recover(capp.AppName, capp.AppIconAddress);
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> AppRegister(AppRegisterAddressModel model)
        {
            bool exists = _dbContext.UserEmails.Any(t => t.EmailAddress == model.Email.ToLower());
            if (exists)
            {
                return this.Protocal(ErrorType.NotEnoughResources, $"A user with email '{model.Email}' already exists!");
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
                    OwnerId = user.Id
                };
                _dbContext.UserEmails.Add(primaryMail);
                await _dbContext.SaveChangesAsync();
                return this.Protocal(ErrorType.Success, "Successfully created your account.");
            }
            return this.Protocal(ErrorType.NotEnoughResources, result.Errors.First().Description);
        }

        [Authorize]
        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return Json(new AiurProtocal { Message = "Successfully signed out!", Code = ErrorType.Success });
        }

        public async Task<IActionResult> UserSignout(UserSignoutAddressModel model)
        {
            await _signInManager.SignOutAsync();
            return Redirect(model.ToRedirect);
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> CodeToOpenId(CodeToOpenIdAddressModel model)
        {
            var AccessToken = await _dbContext.AccessToken.SingleOrDefaultAsync(t => t.Value == model.AccessToken);
            if (AccessToken == null)
            {
                return this.Protocal(ErrorType.WrongKey, "Not a valid access token!");
            }
            else if (!AccessToken.IsAlive)
            {
                return Json(new AiurProtocal { Message = "Access Token is timeout!", Code = ErrorType.Timeout });
            }
            var targetPack = await _dbContext
                .OAuthPack
                .Where(t => t.IsUsed == false)
                .SingleOrDefaultAsync(t => t.Code == model.Code);

            if (targetPack == null)
            {
                return this.Protocal(ErrorType.WrongKey, "Invalid Code.");
            }
            if (targetPack.ApplyAppId != AccessToken.ApplyAppId)
            {
                return this.Protocal(ErrorType.Unauthorized, "The app granted code is not the app granting access token!");
            }
            var capp = (await _apiService.AppInfoAsync(targetPack.ApplyAppId)).App;
            if (capp == null)
            {
                return this.Protocal(ErrorType.NotFound, "App not found.");
            }
            if (!capp.ViewOpenId)
            {
                return this.Protocal(ErrorType.Unauthorized, "The app doesn't have view open id permission.");
            }
            targetPack.IsUsed = true;
            await _dbContext.SaveChangesAsync();
            var viewModel = new CodeToOpenIdViewModel
            {
                openid = targetPack.UserId,
                scope = "scope",
                Message = "Successfully get user openid",
                Code = ErrorType.Success
            };
            return Json(viewModel);
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> UserInfo(UserInfoAddressModel model)
        {
            var target = await _dbContext
                .AccessToken
                .SingleOrDefaultAsync(t => t.Value == model.access_token);

            if (target == null)
            {
                return Json(new AiurProtocal { Message = "Invalid Access Token!", Code = ErrorType.WrongKey });
            }
            else if (!target.IsAlive)
            {
                return Json(new AiurProtocal { Message = "Access Token is timeout!", Code = ErrorType.Timeout });
            }
            var user = await _userManager.FindByIdAsync(model.openid);
            var viewModel = new UserInfoViewModel
            {
                Code = 0,
                Message = "Successfully get target user info.",
                User = user
            };
            return Json(viewModel);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<APIUser> GetUserFromEmail(string email)
        {
            var mail = await _dbContext
                .UserEmails
                .Include(t => t.Owner)
                .SingleOrDefaultAsync(t => t.EmailAddress == email);
            return mail.Owner;
        }

        private async Task<APIUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users
                .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }

        private async Task<IActionResult> FinishAuth(IAuthorizeViewModel model, bool forceGrant = false)
        {
            var user = await GetUserFromEmail(model.Email);
            await _userManager.UpdateAsync(user);
            if (await user.HasAuthorizedApp(_dbContext, model.AppId) && forceGrant == false)
            {
                var pack = await user.GeneratePack(_dbContext, model.AppId);
                var url = new AiurUrl(model.GetRegexRedirectUrl(), new AuthResultAddressModel
                {
                    code = pack.Code,
                    state = model.State
                });
                return Redirect(url);
            }
            else
            {
                return RedirectToAction(nameof(AuthorizeConfirm), new AuthorizeConfirmAddressModel
                {
                    AppId = model.AppId,
                    State = model.State,
                    ToRedirect = model.ToRedirect,
                    Scope = model.Scope,
                    ResponseType = model.ResponseType
                });
            }
        }

        private RedirectResult Redirect(AiurUrl url)
        {
            return Redirect(url.ToString());
        }
    }
}

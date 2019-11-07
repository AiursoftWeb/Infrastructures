using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Gateway.Models.OAuthViewModels;
using Aiursoft.Gateway.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Edi.Captcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Controllers
{
    [LimitPerMin]
    [GenerateDoc]
    [APINotfoundHandler]
    public class OAuthController : Controller
    {
        private readonly UserManager<GatewayUser> _userManager;
        private readonly SignInManager<GatewayUser> _signInManager;
        private readonly ILogger _logger;
        private readonly GatewayDbContext _dbContext;
        private readonly DeveloperApiService _apiService;
        private readonly ConfirmationEmailSender _emailSender;
        private readonly ISessionBasedCaptcha _captcha;
        private readonly UserAppAuthManager _authManager;
        private readonly AuthLogger _authLogger;

        public OAuthController(
            UserManager<GatewayUser> userManager,
            SignInManager<GatewayUser> signInManager,
            ILoggerFactory loggerFactory,
            GatewayDbContext context,
            DeveloperApiService developerApiService,
            ConfirmationEmailSender emailSender,
            ISessionBasedCaptcha sessionBasedCaptcha,
            UserAppAuthManager authManager,
            AuthLogger authLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<OAuthController>();
            _dbContext = context;
            _apiService = developerApiService;
            _emailSender = emailSender;
            _captcha = sessionBasedCaptcha;
            _authManager = authManager;
            _authLogger = authLogger;
        }

        [HttpGet]
        public async Task<IActionResult> Authorize(AuthorizeAddressModel model)
        {
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var url = new Uri(model.RedirectUri);
            var user = await GetCurrentUserAsync();
            // Wrong domain
            if (url.Host != app.AppDomain && app.DebugMode == false)
            {
                ModelState.AddModelError(string.Empty, "Redirect uri did not work in the valid domain!");
                _logger.LogInformation($"A request with appId {model.AppId} is access wrong domian.");
                return View("AuthError");
            }
            // Signed in. App is not in force input password mode. User did not specify force input.
            else if (user != null && app.ForceInputPassword != true && model.ForceConfirm != true)
            {
                await _authLogger.LogAuthRecord(user.Id, HttpContext, true, app.AppId);
                return await _authManager.FinishAuth(user, model, app.ForceConfirmation);
            }
            // Not signed in but we don't want his info
            else if (model.TryAutho == true)
            {
                return Redirect($"{url.Scheme}://{url.Host}:{url.Port}/?{Values.DirectShowString.Key}={Values.DirectShowString.Value}");
            }
            var viewModel = new AuthorizeViewModel(model.RedirectUri, model.State, model.AppId, app.AppName, app.IconPath);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorize(AuthorizeViewModel model)
        {
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            if (!ModelState.IsValid)
            {
                model.Recover(app.AppName, app.IconPath);
                return View(model);
            }
            var mail = await _dbContext
                .UserEmails
                .Include(t => t.Owner)
                .SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
            if (mail == null)
            {
                ModelState.AddModelError(string.Empty, "Unknown user email.");
                model.Recover(app.AppName, app.IconPath);
                return View(model);
            }
            var user = mail.Owner;
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: true);
            await _authLogger.LogAuthRecord(user.Id, HttpContext, result.Succeeded || result.RequiresTwoFactor, app.AppId);
            if (result.Succeeded)
            {
                return await _authManager.FinishAuth(user, model, app.ForceConfirmation);
            }
            if (result.RequiresTwoFactor)
            {
                return Redirect(new AiurUrl($"/oauth/{nameof(SecondAuth)}", new FinishAuthInfo
                {
                    AppId = model.AppId,
                    RedirectUri = model.RedirectUri,
                    State = model.State
                }).ToString());
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "The account is locked for too many attempts.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The password does not match our records.");
            }
            model.Recover(app.AppName, app.IconPath);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AuthorizeConfirm(FinishAuthInfo model)
        {
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            var user = await GetCurrentUserAsync();
            var viewModel = new AuthorizeConfirmViewModel
            {
                AppName = app.AppName,
                UserNickName = user.NickName,
                AppId = model.AppId,
                RedirectUri = model.RedirectUri,
                State = model.State,
                // Permissions
                ViewOpenId = app.ViewOpenId,
                ViewPhoneNumber = app.ViewPhoneNumber,
                ChangePhoneNumber = app.ChangePhoneNumber,
                ConfirmEmail = app.ConfirmEmail,
                ChangeBasicInfo = app.ChangeBasicInfo,
                ChangePassword = app.ChangePassword,
                ChangeGrantInfo = app.ChangeGrantInfo,
                ViewAuditLog = app.ViewAuditLog,
                TermsUrl = app.LicenseUrl,
                PStatementUrl = app.PrivacyStatementUrl,
                ManageSocialAccount = app.ManageSocialAccount
            };
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthorizeConfirm(AuthorizeConfirmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            await _authManager.GrantTargetApp(user, model.AppId);
            return await _authManager.FinishAuth(user, model, false);
        }

        [HttpGet]
        public IActionResult SecondAuth(FinishAuthInfo model)
        {
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var viewModel = new SecondAuthViewModel
            {
                AppId = model.AppId,
                RedirectUri = model.RedirectUri,
                State = model.State
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SecondAuth(SecondAuthViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            var authenticatorCode = model.VerifyCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, true, model.DontAskMeOnIt);
            if (result.Succeeded)
            {
                return await _authManager.FinishAuth(user, model, app.ForceConfirmation);
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "The account is locked for too many attempts.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The code is invalid. Please check and try again.");
            }
            var viewModel = new SecondAuthViewModel
            {
                AppId = model.AppId,
                RedirectUri = model.RedirectUri,
                State = model.State,
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult RecoveryCodeAuth(FinishAuthInfo model)
        {
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var viewModel = new RecoveryCodeAuthViewModel
            {
                AppId = model.AppId,
                RedirectUri = model.RedirectUri,
                State = model.State
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoveryCodeAuth(RecoveryCodeAuthViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
            if (result.Succeeded)
            {
                return await _authManager.FinishAuth(user, model, app.ForceConfirmation);
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "The account is locked for too many attempts.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The code is invalid. Please check and try again.");
            }
            var viewModel = new RecoveryCodeAuthViewModel
            {
                AppId = model.AppId,
                RedirectUri = model.RedirectUri,
                State = model.State,
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Register(AuthorizeAddressModel model)
        {
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var viewModel = new RegisterViewModel(model.RedirectUri, model.State, model.AppId, app.AppName, app.IconPath);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!_captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext.Session))
            {
                ModelState.AddModelError(string.Empty, "Invalid captacha code!");
            }
            var app = (await _apiService.AppInfoAsync(model.AppId)).App;
            if (!ModelState.IsValid)
            {
                model.Recover(app.AppName, app.IconPath);
                return View(model);
            }
            bool exists = _dbContext.UserEmails.Any(t => t.EmailAddress == model.Email.ToLower());
            if (exists)
            {
                ModelState.AddModelError(string.Empty, $"An user with email '{model.Email}' already exists!");
                model.Recover(app.AppName, app.IconPath);
                return View(model);
            }
            var user = new GatewayUser
            {
                UserName = model.Email,
                Email = model.Email,
                NickName = model.Email.Split('@')[0],
                PreferedLanguage = model.PreferedLanguage,
                IconFilePath = Values.DefaultImagePath,
                RegisterIPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
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
                await _emailSender.SendConfirmation(user.Id, primaryMail.EmailAddress, primaryMail.ValidateToken);
                await _authLogger.LogAuthRecord(user.Id, HttpContext, true, app.AppId);
                await _signInManager.SignInAsync(user, isPersistent: true);
                return await _authManager.FinishAuth(user, model, app.ForceConfirmation);
            }
            AddErrors(result);
            model.Recover(app.AppName, app.IconPath);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return Json(new AiurProtocol { Message = "Successfully signed out!", Code = ErrorType.Success });
        }

        public async Task<IActionResult> UserSignout(UserSignoutAddressModel model)
        {
            await _signInManager.SignOutAsync();
            return Redirect(model.ToRedirect);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<GatewayUser> GetCurrentUserAsync()
        {
            return _dbContext
                .Users
                .Include(t => t.Emails)
                .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}

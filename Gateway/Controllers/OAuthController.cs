using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Gateway.Models.OAuthViewModels;
using Aiursoft.Gateway.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Aiursoft.Pylon.Models.API.OAuthViewModels;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Edi.Captcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Controllers
{
    [LimitPerMin]
    [GenerateDoc]
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

        private readonly UserManager<GatewayUser> _userManager;
        private readonly SignInManager<GatewayUser> _signInManager;
        private readonly ILogger _logger;
        private readonly GatewayDbContext _dbContext;
        private readonly DeveloperApiService _apiService;
        private readonly ConfirmationEmailSender _emailSender;
        private readonly ISessionBasedCaptcha _captcha;
        private readonly AuthFinisher _authFinisher;

        public OAuthController(
            UserManager<GatewayUser> userManager,
            SignInManager<GatewayUser> signInManager,
            ILoggerFactory loggerFactory,
            GatewayDbContext context,
            DeveloperApiService developerApiService,
            ConfirmationEmailSender emailSender,
            ISessionBasedCaptcha sessionBasedCaptcha,
            AuthFinisher authFinisher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<OAuthController>();
            _dbContext = context;
            _apiService = developerApiService;
            _emailSender = emailSender;
            _captcha = sessionBasedCaptcha;
            _authFinisher = authFinisher;
        }

        //http://localhost:53657/oauth/authorize?appid=29bf5250a6d93d47b6164ac2821d5009&redirect_uri=http%3A%2F%2Flocalhost%3A55771%2FAuth%2FAuthResult&response_type=code&scope=snsapi_base&state=http%3A%2F%2Flocalhost%3A55771%2FAuth%2FGoAuth#aiursoft_redirect
        [HttpGet]
        public async Task<IActionResult> Authorize(AuthorizeAddressModel model)
        {
            App app;
            try
            {
                app = (await _apiService.AppInfoAsync(model.appid)).App;
            }
            catch (AiurUnexceptedResponse)
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
            // Signed in. App is not in force input password mode. User did not specify force input.
            else if (user != null && app.ForceInputPassword != true && model.forceConfirm != true)
            {
                var log = new AuditLogLocal
                {
                    UserId = user.Id,
                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    Success = true,
                    AppId = app.AppId
                };
                _dbContext.AuditLogs.Add(log);
                await _dbContext.SaveChangesAsync();
                return await _authFinisher.FinishAuth(this, model.Convert(user.Email), app.ForceConfirmation);
            }
            // Not signed in but we don't want his info
            else if (model.tryAutho == true)
            {
                return Redirect($"{url.Scheme}://{url.Host}:{url.Port}/?{Values.DirectShowString.Key}={Values.DirectShowString.Value}");
            }
            var viewModel = new AuthorizeViewModel(model.redirect_uri, model.state, model.appid, model.scope, model.response_type, app.AppName, app.IconPath);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorize(AuthorizeViewModel model)
        {
            App app;
            try
            {
                app = (await _apiService.AppInfoAsync(model.AppId)).App;
            }
            catch (AiurUnexceptedResponse)
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
                model.Recover(app.AppName, app.IconPath);
                return View(model);
            }
            var user = mail.Owner;
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: true);
            var log = new AuditLogLocal
            {
                UserId = user.Id,
                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                Success = result.Succeeded,
                AppId = app.AppId
            };
            _dbContext.AuditLogs.Add(log);
            await _dbContext.SaveChangesAsync();
            if (result.Succeeded)
            {
                return await _authFinisher.FinishAuth(this, model, app.ForceConfirmation);
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
            model.Recover(app.AppName, app.IconPath);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AuthorizeConfirm(AuthorizeConfirmAddressModel model)
        {
            App app;
            try
            {
                app = (await _apiService.AppInfoAsync(model.AppId)).App;
            }
            catch (AiurUnexceptedResponse)
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
                PStatementUrl = app.PrivacyStatementUrl
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
            model.Email = user.Email;
            await user.GrantTargetApp(_dbContext, model.AppId);
            return await _authFinisher.FinishAuth(this, model);
        }

        [HttpGet]
        public async Task<IActionResult> Register(AuthorizeAddressModel model)
        {
            App app;
            try
            {
                app = (await _apiService.AppInfoAsync(model.appid)).App;
            }
            catch (AiurUnexceptedResponse)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View("AuthError");
            }
            var viewModel = new RegisterViewModel(model.redirect_uri, model.state, model.appid, model.scope, model.response_type, app.AppName, app.IconPath);
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
            App app;
            try
            {
                app = (await _apiService.AppInfoAsync(model.AppId)).App;
            }
            catch (AiurUnexceptedResponse)
            {
                return NotFound();
            }
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
                IconFilePath = Values.DefaultImagePath
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
                await _signInManager.SignInAsync(user, isPersistent: true);
                var log = new AuditLogLocal
                {
                    UserId = user.Id,
                    IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    Success = true,
                    AppId = app.AppId
                };
                _dbContext.AuditLogs.Add(log);
                await _dbContext.SaveChangesAsync();
                return await _authFinisher.FinishAuth(this, model);
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

        [AiurNoCache]
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            return _captcha.GenerateCaptchaImageFileStream(HttpContext.Session, 100, 33);
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

        private RedirectResult Redirect(AiurUrl url)
        {
            return Redirect(url.ToString());
        }
    }
}

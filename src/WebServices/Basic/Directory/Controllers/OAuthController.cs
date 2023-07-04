using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Canon;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.Models;
using Aiursoft.Directory.Models.OAuthViewModels;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Directory.Services;
using Aiursoft.Identity;
using Edi.Captcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Directory.Controllers;

public class OAuthController : Controller
{
    private readonly bool _allowPasswordSignIn;
    private readonly bool _allowRegistering;
    private readonly AuthLogger _authLogger;
    private readonly UserAppAuthManager _authManager;
    private readonly CanonService _cannonService;
    private readonly ISessionBasedCaptcha _captcha;
    private readonly DirectoryDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly SignInManager<DirectoryUser> _signInManager;
    private readonly UserManager<DirectoryUser> _userManager;

    public OAuthController(
        UserManager<DirectoryUser> userManager,
        SignInManager<DirectoryUser> signInManager,
        ILoggerFactory loggerFactory,
        DirectoryDbContext context,
        ISessionBasedCaptcha sessionBasedCaptcha,
        UserAppAuthManager authManager,
        AuthLogger authLogger,
        IConfiguration configuration,
        CanonService cannonService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<OAuthController>();
        _dbContext = context;
        _captcha = sessionBasedCaptcha;
        _authManager = authManager;
        _authLogger = authLogger;
        _cannonService = cannonService;
        _allowRegistering = configuration["AllowSelfRegistering"].Trim().ToLower() == true.ToString().ToLower();
        _allowPasswordSignIn = configuration["AllowPasswordSignIn"].Trim().ToLower() == true.ToString().ToLower();
    }

    [HttpGet]
    public async Task<IActionResult> Authorize(AuthorizeAddressModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("AuthError");
        }

        var app = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (app == null)
        {
            ModelState.AddModelError(string.Empty, $"App with ID: {model.AppId} was not found!");
            _logger.LogInformation("A request with appId {AppId} is access wrong domain", model.AppId);
            return View("AuthError");
        }
        
        var url = new Uri(model.RedirectUri);
        var user = await GetCurrentUserAsync();
        // Wrong domain
        if (url.Host != app.AppDomain && app.DebugMode == false)
        {
            ModelState.AddModelError(string.Empty, "Redirect uri did not work in the valid domain!");
            _logger.LogInformation("A request with appId {AppId} is access wrong domain", model.AppId);
            return View("AuthError");
        }
        // Signed in. App is not in force input password mode. User did not specify force input.

        if (user != null && app.ForceInputPassword != true && model.ForceConfirm != true)
        {
            await _authLogger.LogAuthRecord(user.Id, HttpContext, true, app.AppId);
            return await _authManager.FinishAuth(user, model, app.ForceConfirmation, app.TrustedApp);
        }
        // Not signed in but we don't want his info

        if (model.TryAutho == true)
        {
            return Redirect(
                $"{url.Scheme}://{url.Host}:{url.Port}/?{AuthValues.DirectShowString.Key}={AuthValues.DirectShowString.Value}");
        }

        var viewModel = new AuthorizeViewModel(model.RedirectUri, model.State, model.AppId, app.AppName, app.IconPath,
            _allowRegistering, _allowPasswordSignIn);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Authorize(AuthorizeViewModel model)
    {
        if (!_allowPasswordSignIn)
        {
            return Unauthorized();
        }

        var app = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (app == null)
        {
            ModelState.AddModelError(string.Empty, $"App with ID: {model.AppId} was not found!");
            _logger.LogInformation("A request with appId {AppId} is access wrong domain", model.AppId);
            return View("AuthError");
        }
        
        if (!ModelState.IsValid)
        {
            model.Recover(app.AppName, app.IconPath, _allowRegistering, _allowPasswordSignIn);
            return View(model);
        }

        var mail = await _dbContext
            .UserEmails
            .Include(t => t.Owner)
            .SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
        if (mail == null)
        {
            ModelState.AddModelError(string.Empty, "Unknown user email.");
            model.Recover(app.AppName, app.IconPath, _allowRegistering, _allowPasswordSignIn);
            return View(model);
        }

        var user = mail.Owner;
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
        await _authLogger.LogAuthRecord(user.Id, HttpContext, result.Succeeded || result.RequiresTwoFactor, app.AppId);
        if (result.Succeeded)
        {
            return await _authManager.FinishAuth(user, model, app.ForceConfirmation, app.TrustedApp);
        }

        if (result.RequiresTwoFactor)
        {
            return this.Protocol(new AiurApiEndpoint(string.Empty, "OAuth", nameof(SecondAuth), new
            {
                model.AppId,
                model.RedirectUri,
                model.State
            }));
        }

        ModelState.AddModelError(string.Empty,
            result.IsLockedOut
                ? "The account is locked for too many attempts."
                : "The password does not match our records.");
        model.Recover(app.AppName, app.IconPath, _allowRegistering, _allowPasswordSignIn);
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

        var app = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (app == null)
        {
            ModelState.AddModelError(string.Empty, $"App with ID: {model.AppId} was not found!");
            _logger.LogInformation("A request with appId {AppId} is access wrong domain", model.AppId);
            return View("AuthError");
        }
        
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
        return await _authManager.FinishAuth(user, model, false, false);
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
        var app = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (app == null)
        {
            ModelState.AddModelError(string.Empty, $"App with ID: {model.AppId} was not found!");
            _logger.LogInformation("App with ID: {ModelAppId} was not found!", model.AppId);
            return View("AuthError");
        }
        
        var authenticatorCode = model.VerifyCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result =
            await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, true, model.DoNotAskMeOnIt);
        if (result.Succeeded)
        {
            return await _authManager.FinishAuth(user, model, app.ForceConfirmation, app.TrustedApp);
        }

        ModelState.AddModelError(string.Empty,
            result.IsLockedOut
                ? "The account is locked for too many attempts."
                : "The code is invalid. Please check and try again.");

        var viewModel = new SecondAuthViewModel
        {
            AppId = model.AppId,
            RedirectUri = model.RedirectUri,
            State = model.State
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
        var app = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (app == null)
        {
            ModelState.AddModelError(string.Empty, $"App with ID: {model.AppId} was not found!");
            _logger.LogInformation("A request with appId {AppId} is access wrong domain", model.AppId);
            return View("AuthError");
        }
        
        var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        if (result.Succeeded)
        {
            return await _authManager.FinishAuth(user, model, app.ForceConfirmation, app.TrustedApp);
        }

        ModelState.AddModelError(string.Empty,
            result.IsLockedOut
                ? "The account is locked for too many attempts."
                : "The code is invalid. Please check and try again.");

        var viewModel = new RecoveryCodeAuthViewModel
        {
            AppId = model.AppId,
            RedirectUri = model.RedirectUri,
            State = model.State
        };
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Register(AuthorizeAddressModel model)
    {
        if (!_allowRegistering)
        {
            return Unauthorized();
        }

        var app = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (app == null)
        {
            ModelState.AddModelError(string.Empty, $"App with ID: {model.AppId} was not found!");
            _logger.LogInformation("App with ID: {ModelAppId} was not found!", model.AppId);
            return View("AuthError");
        }
        
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
        if (!_allowRegistering)
        {
            return Unauthorized();
        }

        if (!_captcha.Validate(model.CaptchaCode, HttpContext.Session))
        {
            ModelState.AddModelError(string.Empty, "Invalid captcha code!");
        }

        var app = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (app == null)
        {
            ModelState.AddModelError(string.Empty, $"App with ID: {model.AppId} was not found!");
            _logger.LogInformation("App with ID: {ModelAppId} was not found!", model.AppId);
            return View("AuthError");
        }
        
        if (!ModelState.IsValid)
        {
            model.Recover(app.AppName, app.IconPath);
            return View(model);
        }

        var exists = _dbContext.UserEmails.Any(t => t.EmailAddress == model.Email.ToLower());
        if (exists)
        {
            ModelState.AddModelError(string.Empty, $"An user with email '{model.Email}' already exists!");
            model.Recover(app.AppName, app.IconPath);
            return View(model);
        }

        var countStart = DateTime.UtcNow - TimeSpan.FromDays(1);
        var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (await _dbContext.Users
                .Where(t => t.RegisterIPAddress == requestIp)
                .Where(t => t.AccountCreateTime > countStart)
                .CountAsync() > 5)
        {
            ModelState.AddModelError(string.Empty, "You can't create more than 5 accounts in one day!");
            model.Recover(app.AppName, app.IconPath);
            return View(model);
        }

        var user = new DirectoryUser
        {
            UserName = model.Email,
            Email = model.Email,
            NickName = model.Email.Split('@')[0],
            PreferedLanguage = model.PreferedLanguage,
            IconFilePath = AuthValues.DefaultImagePath,
            RegisterIPAddress = requestIp
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            var primaryMail = new UserEmail
            {
                EmailAddress = model.Email.ToLower(),
                OwnerId = user.Id,
                ValidateToken = Guid.NewGuid().ToString("N"),
                LastSendTime = DateTime.UtcNow
            };
            await _dbContext.UserEmails.AddAsync(primaryMail);
            await _dbContext.SaveChangesAsync();
            // Send him an confirmation email here:
            _cannonService.FireAsync<ConfirmationEmailSender>(async sender =>
            {
                await sender.SendConfirmation(user.Id, primaryMail.EmailAddress, primaryMail.ValidateToken);
            });
            await _authLogger.LogAuthRecord(user.Id, HttpContext, true, app.AppId);
            await _signInManager.SignInAsync(user, true);
            return await _authManager.FinishAuth(user, model, app.ForceConfirmation, app.TrustedApp);
        }

        AddErrors(result);
        model.Recover(app.AppName, app.IconPath);
        return View(model);
    }

    public async Task<IActionResult> UserSignOut(UserSignOutAddressModel model)
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

    private Task<DirectoryUser> GetCurrentUserAsync()
    {
        return _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
    }
}
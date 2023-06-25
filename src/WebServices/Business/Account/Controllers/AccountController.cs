using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Account.Models;
using Aiursoft.Account.Models.AccountViewModels;
using Aiursoft.Account.Services;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.Canon;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Directory.SDK.Services.ToDirectoryServer;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Identity.Attributes;
using Aiursoft.Identity.Services;
using Aiursoft.Identity.Services.Authentication;
using Aiursoft.WebTools.Data;
using Aiursoft.WebTools.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.Account.Controllers;

[AiurForceAuth]
public class AccountController : Controller
{
    private readonly DirectoryAppTokenService _directoryAppTokenService;
    private readonly IEnumerable<IAuthProvider> _authProviders;
    private readonly AuthService<AccountUser> _authService;
    private readonly CanonService _cannonService;
    private readonly IConfiguration _configuration;
    private readonly QRCodeService _qrCodeService;
    private readonly CanonPool _canonPool;
    private readonly AppsService _appsService;
    private readonly UserManager<AccountUser> _userManager;
    private readonly UserService _userService;

    public AccountController(
        CanonPool canonPool,
        AppsService appsService,
        UserManager<AccountUser> userManager,
        UserService userService,
        DirectoryAppTokenService directoryAppTokenService,
        IConfiguration configuration,
        AuthService<AccountUser> authService,
        IEnumerable<IAuthProvider> authProviders,
        QRCodeService qrCodeService,
        CanonService cannonService)
    {
        _canonPool = canonPool;
        _appsService = appsService;
        _userManager = userManager;
        _userService = userService;
        _directoryAppTokenService = directoryAppTokenService;
        _configuration = configuration;
        _authService = authService;
        _authProviders = authProviders;
        _qrCodeService = qrCodeService;
        _cannonService = cannonService;
    }

    public async Task<IActionResult> Index(bool? justHaveUpdated)
    {
        var user = await GetCurrentUserAsync();
        var model = new IndexViewModel(user)
        {
            JustHaveUpdated = justHaveUpdated ?? false
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IndexViewModel model)
    {
        var currentUser = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            model.Recover(currentUser);
            return View(model);
        }

        currentUser.NickName = model.NickName;
        currentUser.Bio = model.Bio;
        await _userService.ChangeProfileAsync(currentUser.Id, await _directoryAppTokenService.GetAccessTokenAsync(),
            currentUser.NickName, currentUser.IconFilePath, currentUser.Bio);
        await _userManager.UpdateAsync(currentUser);
        return RedirectToAction(nameof(Index), new { JustHaveUpdated = true });
    }

    [HttpGet]
    public async Task<IActionResult> Email(bool justHaveUpdated)
    {
        var user = await GetCurrentUserAsync();
        user = await _authService.Fetch(user);
        var emails = await _userService.ViewAllEmailsAsync(await _directoryAppTokenService.GetAccessTokenAsync(), user.Id);
        var model = new EmailViewModel(user)
        {
            Emails = emails.Items,
            PrimaryEmail = user.Email,
            JustHaveUpdated = justHaveUpdated
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Email(EmailViewModel model)
    {
        var user = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            model.Recover(user);
            return View(model);
        }

        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        try
        {
            await _userService.BindNewEmailAsync(user.Id, model.NewEmail, token);
        }
        catch (AiurUnexpectedServerResponseException e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            model.Recover(user);
            var emails = await _userService.ViewAllEmailsAsync(await _directoryAppTokenService.GetAccessTokenAsync(), user.Id);
            model.Emails = emails.Items;
            model.PrimaryEmail = user.Email;
            return View(model);
        }

        return RedirectToAction(nameof(Email), new { JustHaveUpdated = true });
    }

    [HttpPost]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> SendEmail([EmailAddress] string email)
    {
        var user = await GetCurrentUserAsync();
        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        var result = await _userService.SendConfirmationEmailAsync(token, user.Id, email);
        return this.Protocol(result);
    }

    [HttpPost]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> DeleteEmail([EmailAddress] string email)
    {
        var user = await GetCurrentUserAsync();
        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        var result = await _userService.DeleteEmailAsync(user.Id, email, token);
        return this.Protocol(result);
    }

    [HttpPost]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> SetPrimaryEmail([EmailAddress] string email)
    {
        var user = await GetCurrentUserAsync();
        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        var result = await _userService.SetPrimaryEmailAsync(token, user.Id, email);
        return this.Protocol(result);
    }

    public async Task<IActionResult> Avatar(bool justHaveUpdated)
    {
        var user = await GetCurrentUserAsync();
        var model = new AvatarViewModel(user)
        {
            JustHaveUpdated = justHaveUpdated,
            NewIconAddress = user.IconFilePath
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Avatar(AvatarViewModel model)
    {
        var currentUser = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            model.Recover(currentUser);
            return View(model);
        }

        currentUser.IconFilePath = model.NewIconAddress;
        await _userService.ChangeProfileAsync(currentUser.Id, await _directoryAppTokenService.GetAccessTokenAsync(),
            currentUser.NickName, currentUser.IconFilePath, currentUser.Bio);
        await _userManager.UpdateAsync(currentUser);
        return RedirectToAction(nameof(Avatar), new { JustHaveUpdated = true });
    }

    public async Task<IActionResult> Security(bool justHaveUpdated)
    {
        var user = await GetCurrentUserAsync();
        var model = new SecurityViewModel(user)
        {
            JustHaveUpdated = justHaveUpdated
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Security(SecurityViewModel model)
    {
        var currentUser = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            model.Recover(currentUser);
            return View(model);
        }

        try
        {
            await _userService.ChangePasswordAsync(currentUser.Id, await _directoryAppTokenService.GetAccessTokenAsync(),
                model.OldPassword, model.NewPassword);
            return RedirectToAction(nameof(Security), new { JustHaveUpdated = true });
        }
        catch (AiurUnexpectedServerResponseException e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            model.Recover(currentUser);
        }

        return View(model);
    }

    public async Task<IActionResult> Phone(bool justHaveUpdated)
    {
        var user = await GetCurrentUserAsync();
        var phone = await _userService.ViewPhoneNumberAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync());
        var model = new PhoneViewModel(user)
        {
            CurrentPhoneNumber = phone.Value,
            PhoneNumberConfirmed = !string.IsNullOrEmpty(phone.Value),
            JustHaveUpdated = justHaveUpdated,
            AvailableZoneNumbers = new SelectList(
                ZoneNumbers.Numbers.Select(t => new KeyValuePair<string, string>($"+{t.Value} {t.Key}", "+" + t.Value)),
                nameof(KeyValuePair<string, string>.Value),
                nameof(KeyValuePair<string, string>.Key),
                ZoneNumbers.Numbers.First().Value)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Phone(PhoneViewModel model)
    {
        var user = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            model.Recover(user);
            return View(model);
        }

        var phone = model.ZoneNumber + model.NewPhoneNumber;
        
        // TODO: Migrate the logic to directory!
        var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phone);
        _cannonService.FireAsync<AccountSmsSender>(async sender =>
        {
            await sender.SendAsync(phone, $"Your Aiursoft verification code is: {code}.");
        });
        return RedirectToAction(nameof(EnterCode), new { newPhoneNumber = phone });
    }

    public async Task<IActionResult> EnterCode(string newPhoneNumber)
    {
        var user = await GetCurrentUserAsync();
        var model = new EnterCodeViewModel(user)
        {
            NewPhoneNumber = newPhoneNumber
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnterCode(EnterCodeViewModel model)
    {
        var user = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            model.Recover(user);
            return View(model);
        }

        var correctToken = await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, model.NewPhoneNumber);
        if (correctToken)
        {
            await _userService.SetPhoneNumberAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync(),
                model.NewPhoneNumber);

            user.PhoneNumber = model.NewPhoneNumber;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Phone), new { JustHaveUpdated = true });
        }

        model.Recover(user);
        ModelState.AddModelError(string.Empty, "Your token is invalid!");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnBind()
    {
        var user = await GetCurrentUserAsync();
        await _userService.SetPhoneNumberAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync(), string.Empty);

        user.PhoneNumber = string.Empty;
        await _userManager.UpdateAsync(user);
        return RedirectToAction(nameof(Phone));
    }

    public async Task<IActionResult> Applications()
    {
        var user = await GetCurrentUserAsync();
        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        var model = new ApplicationsViewModel(user)
        {
            Grants = (await _userService.ViewGrantedAppsAsync(token, user.Id)).Items
        };
        var appsBag = new ConcurrentBag<DirectoryApp>();
        foreach (var grant in model.Grants ?? Array.Empty<Grant>())
        {
            _canonPool.RegisterNewTaskToPool(async () =>
            {
                try
                {
                    var appInfo = await _appsService.AppInfoAsync(grant.AppId);
                    appsBag.Add(appInfo.App);
                }
                catch (AiurUnexpectedServerResponseException e) when (e.Response.Code == Code.NotFound) { }
            });
        }
        await _canonPool.RunAllTasksInPoolAsync();
        model.Apps = appsBag.OrderBy(app =>
            model.Grants.Single(grant => grant.AppId == app.AppId).GrantTime).ToList();
        return View(model);
    }

    [HttpPost]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> DeleteGrant(string appId)
    {
        var user = await GetCurrentUserAsync();
        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        if (_configuration["AccountAppId"] == appId)
        {
            return this.Protocol(Code.InvalidInput, "You can not revoke Aiursoft Account Center!");
        }

        var result = await _userService.DropGrantedAppsAsync(token, user.Id, appId);
        return this.Protocol(result);
    }

    public async Task<IActionResult> AuditLog(int page = 1)
    {
        var user = await GetCurrentUserAsync();
        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        var logs = await _userService.ViewAuditLogAsync(token, user.Id, page);
        var model = new AuditLogViewModel(user)
        {
            Logs = logs
        };
        foreach (var id in model.Logs.Items?.Select(t => t.AppId).Distinct() ?? Array.Empty<string>())
        {
            _canonPool.RegisterNewTaskToPool(async () =>
            {
                var appInfo = await _appsService.AppInfoAsync(id);
                model.Apps.Add(appInfo.App);
            });
        }
        await _canonPool.RunAllTasksInPoolAsync();
        return View(model);
    }

    public async Task<IActionResult> TwoFactorAuthentication()
    {
        var user = await GetCurrentUserAsync();
        var has2FaKey = await _userService.ViewHas2FAKeyAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync());
        var twoFactorEnabled =
            await _userService.ViewTwoFactorEnabledAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync());
        var model = new TwoFactorAuthenticationViewModel(user)
        {
            NewHas2FAKey = has2FaKey.Value,
            NewTwoFactorEnabled = twoFactorEnabled.Value
        };
        return View(model);
    }

    public async Task<IActionResult> ViewTwoFAKey()
    {
        var user = await GetCurrentUserAsync();
        var key = await _userService.View2FAKeyAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync());
        var model = new View2FAKeyViewModel(user)
        {
            NewTwoFAKey = key.TwoFAKey,
            QRCodeBase64 = _qrCodeService.ToQRCodeBase64(key.TwoFAQRUri)
        };
        return View(model);
    }

    public async Task<IActionResult> SetTwoFAKey()
    {
        var user = await GetCurrentUserAsync();
        await _userService.SetTwoFAKeyAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync());
        return RedirectToAction(nameof(ViewTwoFAKey));
    }

    public async Task<IActionResult> ResetTwoFAKey()
    {
        var user = await GetCurrentUserAsync();
        await _userService.ResetTwoFAKeyAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync());
        return RedirectToAction(nameof(ViewTwoFAKey));
    }

    public async Task<IActionResult> VerifyTwoFACode()
    {
        var user = await GetCurrentUserAsync();
        var model = new VerifyTwoFACodeViewModel(user);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyTwoFACode(VerifyTwoFACodeViewModel model)
    {
        var user = await GetCurrentUserAsync();
        var success =
            (await _userService.TwoFAVerifyCodeAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync(), model.Code))
            .Value;
        if (success)
            // go to recovery codes page
        {
            return RedirectToAction(nameof(TwoFactorAuthentication), new { success = true });
        }

        ModelState.AddModelError(string.Empty, "Invalid code!");
        model.RootRecover(user, "Two-factor Authentication");
        return View(model);
    }

    public async Task<IActionResult> DisableTwoFA()
    {
        var user = await GetCurrentUserAsync();
        var model = new DisableTwoFAViewModel(user);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableTwoFA(DisableTwoFAViewModel _)
    {
        var user = await GetCurrentUserAsync();
        var disableResult = await _userService.DisableTwoFAAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync());
        if (disableResult.Value)
        {
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        throw new InvalidOperationException("Disable two FA crashed!");
    }

    public async Task<IActionResult> GetRecoveryCodes(GetRecoveryCodesViewModel model)
    {
        var user = await GetCurrentUserAsync();
        var newCodesKey = await _userService.GetRecoveryCodesAsync(user.Id, await _directoryAppTokenService.GetAccessTokenAsync());
        model.NewRecoveryCodesKey = newCodesKey.Items?.ToList();
        model.RootRecover(user, "Two-factor Authentication");
        return View(model);
    }

    public async Task<IActionResult> Social()
    {
        var user = await GetCurrentUserAsync();
        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        var model = new SocialViewModel(user)
        {
            Accounts = (await _userService.ViewSocialAccountsAsync(token, user.Id)).Items?.ToList(),
            Providers = _authProviders
        };
        return View(model);
    }

    [HttpPost]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> UnBindAccount([Required]string provider)
    {
        var user = await GetCurrentUserAsync();
        var token = await _directoryAppTokenService.GetAccessTokenAsync();
        var result = await _userService.UnBindSocialAccountAsync(token, user.Id, provider);
        return this.Protocol(result);
    }

    private async Task<AccountUser> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(User);
    }
}
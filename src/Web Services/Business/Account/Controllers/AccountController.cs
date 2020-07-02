using Aiursoft.Account.Models;
using Aiursoft.Account.Models.AccountViewModels;
using Aiursoft.Account.Services;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Developer.SDK.Models;
using Aiursoft.Developer.SDK.Services.ToDeveloperServer;
using Aiursoft.Gateway.SDK.Services.ToGatewayServer;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Identity.Attributes;
using Aiursoft.Identity.Services;
using Aiursoft.Identity.Services.Authentication;
using Aiursoft.WebTools;
using Aiursoft.WebTools.Data;
using Aiursoft.WebTools.Services;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Controllers
{
    [LimitPerMin]
    [AiurForceAuth]
    public class AccountController : Controller
    {
        private readonly UserManager<AccountUser> _userManager;
        private readonly AccountSmsSender _smsSender;
        private readonly UserService _userService;
        private readonly AppsContainer _appsContainer;
        private readonly IConfiguration _configuration;
        private readonly DeveloperApiService _developerApiService;
        private readonly AuthService<AccountUser> _authService;
        private readonly IEnumerable<IAuthProvider> _authProviders;
        private readonly QRCodeService _qrCodeService;
        private readonly AiurCache _cache;

        public AccountController(
            UserManager<AccountUser> userManager,
            AccountSmsSender smsSender,
            UserService userService,
            AppsContainer appsContainer,
            IConfiguration configuration,
            DeveloperApiService developerApiService,
            AuthService<AccountUser> authService,
            IEnumerable<IAuthProvider> authProviders,
            QRCodeService qrCodeService,
            AiurCache cache)
        {
            _userManager = userManager;
            _smsSender = smsSender;
            _userService = userService;
            _appsContainer = appsContainer;
            _configuration = configuration;
            _developerApiService = developerApiService;
            _authService = authService;
            _authProviders = authProviders;
            _qrCodeService = qrCodeService;
            _cache = cache;
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
                model.ModelStateValid = false;
                model.Recover(currentUser);
                return View(model);
            }
            currentUser.NickName = model.NickName;
            currentUser.Bio = model.Bio;
            await _userService.ChangeProfileAsync(currentUser.Id, await _appsContainer.AccessToken(), currentUser.NickName, currentUser.IconFilePath, currentUser.Bio);
            await _userManager.UpdateAsync(currentUser);
            return RedirectToAction(nameof(Index), new { JustHaveUpdated = true });
        }

        [HttpGet]
        public async Task<IActionResult> Email(bool justHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            user = await _authService.OnlyUpdate(user);
            var emails = await _userService.ViewAllEmailsAsync(await _appsContainer.AccessToken(), user.Id);
            var model = new EmailViewModel(user)
            {
                Emails = emails.Items,
                PrimaryEmail = user.Email
            };
            model.JustHaveUpdated = justHaveUpdated;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Email(EmailViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            var token = await _appsContainer.AccessToken();
            try
            {
                await _userService.BindNewEmailAsync(user.Id, model.NewEmail, token);
            }
            catch (AiurUnexpectedResponse e)
            {
                model.ModelStateValid = false;
                ModelState.AddModelError(string.Empty, e.Message);
                model.Recover(user);
                var emails = await _userService.ViewAllEmailsAsync(await _appsContainer.AccessToken(), user.Id);
                model.Emails = emails.Items;
                model.PrimaryEmail = user.Email;
                return View(model);
            }
            return RedirectToAction(nameof(Email), new { JustHaveUpdated = true });
        }

        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> SendEmail([EmailAddress]string email)
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var result = await _userService.SendConfirmationEmailAsync(token, user.Id, email);
            return Json(result);
        }

        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> DeleteEmail([EmailAddress]string email)
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var result = await _userService.DeleteEmailAsync(user.Id, email, token);
            return Json(result);
        }

        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> SetPrimaryEmail([EmailAddress]string email)
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var result = await _userService.SetPrimaryEmailAsync(token, user.Id, email);
            return Json(result);
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
                model.ModelStateValid = false;
                model.Recover(currentUser);
                return View(model);
            }
            currentUser.IconFilePath = model.NewIconAddress;
            await _userService.ChangeProfileAsync(currentUser.Id, await _appsContainer.AccessToken(), currentUser.NickName, currentUser.IconFilePath, currentUser.Bio);
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
                model.ModelStateValid = false;
                model.Recover(currentUser);
                return View(model);
            }
            try
            {
                await _userService.ChangePasswordAsync(currentUser.Id, await _appsContainer.AccessToken(), model.OldPassword, model.NewPassword);
                return RedirectToAction(nameof(Security), new { JustHaveUpdated = true });
            }
            catch (AiurUnexpectedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                model.ModelStateValid = false;
                model.Recover(currentUser);
            }
            return View(model);
        }

        public async Task<IActionResult> Phone(bool justHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var phone = await _userService.ViewPhoneNumberAsync(user.Id, await _appsContainer.AccessToken());
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
                model.ModelStateValid = ModelState.IsValid;
                return View(model);
            }
            var phone = model.ZoneNumber + model.NewPhoneNumber;
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phone);
            await _smsSender.SendAsync(phone, $"Your Aiursoft verification code is: {code}.");
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
                model.ModelStateValid = ModelState.IsValid;
                return View(model);
            }
            var correctToken = await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, model.NewPhoneNumber);
            if (correctToken)
            {
                var result = await _userService.SetPhoneNumberAsync(user.Id, await _appsContainer.AccessToken(), model.NewPhoneNumber);
                if (result.Code == ErrorType.Success)
                {
                    user.PhoneNumber = model.NewPhoneNumber;
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction(nameof(Phone), new { JustHaveUpdated = true });
                }
                throw new InvalidOperationException();
            }
            else
            {
                model.ModelStateValid = false;
                model.Recover(user);
                ModelState.AddModelError(string.Empty, "Your token is invalid!");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnBind()
        {
            var user = await GetCurrentUserAsync();
            var result = await _userService.SetPhoneNumberAsync(user.Id, await _appsContainer.AccessToken(), string.Empty);
            if (result.Code == ErrorType.Success)
            {
                user.PhoneNumber = string.Empty;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Phone));
            }
            throw new InvalidOperationException();
        }

        public async Task<IActionResult> Applications()
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var model = new ApplicationsViewModel(user)
            {
                Grants = (await _userService.ViewGrantedAppsAsync(token, user.Id)).Items
            };
            var appsBag = new ConcurrentBag<App>();
            await model.Grants.ForEachParallel(async grant =>
            {
                try
                {
                    var appInfo = await _developerApiService.AppInfoAsync(grant.AppId);
                    appsBag.Add(appInfo.App);
                }
                catch (AiurUnexpectedResponse e) when (e.Code == ErrorType.NotFound) { }
            });
            model.Apps = appsBag.OrderBy(app =>
                model.Grants.Single(grant => grant.AppId == app.AppId).GrantTime).ToList();
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> DeleteGrant(string appId)
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            if (_configuration["AccountAppId"] == appId)
            {
                return this.Protocol(ErrorType.InvalidInput, "You can not revoke Aiursoft Account Center!");
            }
            var result = await _userService.DropGrantedAppsAsync(token, user.Id, appId);
            return Json(result);
        }

        public async Task<IActionResult> AuditLog(int page = 1)
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var logs = await _userService.ViewAuditLogAsync(token, user.Id, page);
            var model = new AuditLogViewModel(user)
            {
                Logs = logs
            };
            await model.Logs.Items.Select(t => t.AppId).Distinct().ForEachParallel(async (id) =>
            {
                var appInfo = await _cache.GetAndCache($"appInfo-{id}", () => _developerApiService.AppInfoAsync(id));
                model.Apps.Add(appInfo.App);
            });
            return View(model);
        }

        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            var has2FAKey = await _userService.ViewHas2FAKeyAsync(user.Id, await _appsContainer.AccessToken());
            var twoFactorEnabled = await _userService.ViewTwoFactorEnabledAsync(user.Id, await _appsContainer.AccessToken());
            var model = new TwoFactorAuthenticationViewModel(user)
            {
                NewHas2FAKey = has2FAKey.Value,
                NewTwoFactorEnabled = twoFactorEnabled.Value
            };
            return View(model);
        }

        public async Task<IActionResult> ViewTwoFAKey()
        {
            var user = await GetCurrentUserAsync();
            var key = await _userService.View2FAKeyAsync(user.Id, await _appsContainer.AccessToken());
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
            await _userService.SetTwoFAKeyAsync(user.Id, await _appsContainer.AccessToken());
            return RedirectToAction(nameof(ViewTwoFAKey));
        }

        public async Task<IActionResult> ResetTwoFAKey()
        {
            var user = await GetCurrentUserAsync();
            await _userService.ResetTwoFAKeyAsync(user.Id, await _appsContainer.AccessToken());
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
            var success = (await _userService.TwoFAVerifyCodeAsync(user.Id, await _appsContainer.AccessToken(), model.Code)).Value;
            if (success)
            {
                // go to recovery codes page
                return RedirectToAction(nameof(TwoFactorAuthentication), new { success = true });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code!");
                model.RootRecover(user, "Two-factor Authentication");
                return View(model);
            }
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
            var disableResult = await _userService.DisableTwoFAAsync(user.Id, await _appsContainer.AccessToken());
            if (disableResult.Value)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }
            else
            {
                throw new InvalidOperationException("Disable two FA crashed!");
            }
        }

        public async Task<IActionResult> GetRecoveryCodes(GetRecoveryCodesViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var newCodesKey = await _userService.GetRecoveryCodesAsync(user.Id, await _appsContainer.AccessToken());
            model.NewRecoveryCodesKey = newCodesKey.Items;
            model.RootRecover(user, "Two-factor Authentication");
            return View(model);
        }

        public async Task<IActionResult> Social()
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var model = new SocialViewModel(user)
            {
                Accounts = (await _userService.ViewSocialAccountsAsync(token, user.Id)).Items,
                Providers = _authProviders
            };
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> UnBindAccount(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return this.Protocol(ErrorType.Success, "Seems no this provider at all...");
            }
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var result = await _userService.UnBindSocialAccountAsync(token, user.Id, provider);
            return Json(result);
        }

        private async Task<AccountUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}

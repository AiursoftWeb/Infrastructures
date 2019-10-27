using Aiursoft.Account.Models;
using Aiursoft.Account.Models.AccountViewModels;
using Aiursoft.Account.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.Authentication;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Aiursoft.Pylon.Services.ToGatewayServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
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
        private readonly AiurCache _cache;

        public AccountController(
            UserManager<AccountUser> userManager,
            AccountSmsSender smsSender,
            UserService userService,
            AppsContainer appsContainer,
            IConfiguration configuration,
            DeveloperApiService developerApiSerivce,
            AuthService<AccountUser> authService,
            IEnumerable<IAuthProvider> authProviders,
            AiurCache cache)
        {
            _userManager = userManager;
            _smsSender = smsSender;
            _userService = userService;
            _appsContainer = appsContainer;
            _configuration = configuration;
            _developerApiService = developerApiSerivce;
            _authService = authService;
            _authProviders = authProviders;
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
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(cuser);
                return View(model);
            }
            cuser.NickName = model.NickName;
            cuser.Bio = model.Bio;
            await _userService.ChangeProfileAsync(cuser.Id, await _appsContainer.AccessToken(), cuser.NickName, cuser.IconFilePath, cuser.Bio);
            await _userManager.UpdateAsync(cuser);
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
            catch (AiurUnexceptedResponse e)
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
                NewIconAddres = user.IconFilePath
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Avatar(AvatarViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(cuser);
                return View(model);
            }
            cuser.IconFilePath = model.NewIconAddres;
            await _userService.ChangeProfileAsync(cuser.Id, await _appsContainer.AccessToken(), cuser.NickName, cuser.IconFilePath, cuser.Bio);
            await _userManager.UpdateAsync(cuser);
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
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(cuser);
                return View(model);
            }
            try
            {
                await _userService.ChangePasswordAsync(cuser.Id, await _appsContainer.AccessToken(), model.OldPassword, model.NewPassword);
                return RedirectToAction(nameof(Security), new { JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                model.ModelStateValid = false;
                model.Recover(cuser);
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
                AvailableZoneNumbers = ZoneNumbers.BuildSelectList()
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
            var taskList = new List<Task>();
            foreach (var app in model.Grants)
            {
                async Task AddApp()
                {
                    var appInfo = await _developerApiService.AppInfoAsync(app.AppID);
                    model.Apps.Add(appInfo.App);
                }
                taskList.Add(AddApp());
            }
            await Task.WhenAll(taskList);
            model.Apps = model.Apps.OrderBy(app =>
                model.Grants.Single(grant => grant.AppID == app.AppId).GrantTime).ToList();
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

        public async Task<IActionResult> AuditLog()
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var model = new AuditLogViewModel(user)
            {
                Logs = (await _userService.ViewAuditLogAsync(token, user.Id)).Items
            };
            await model.Logs.Select(t => t.AppId).Distinct().ForEachParallel(async (id) =>
            {
                var appInfo = await _cache.GetAndCache($"appInfo-{id}", () => _developerApiService.AppInfoAsync(id));
                model.Apps.Add(appInfo.App);
            });
            return View(model);
        }

        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            var has2FAkey = await _userService.ViewHas2FAkeyAsync(user.Id, await _appsContainer.AccessToken());
            var twoFactorEnabled = await _userService.ViewTwoFactorEnabledAsync(user.Id, await _appsContainer.AccessToken());
            var model = new TwoFactorAuthenticationViewModel(user)
            {
                NewHas2FAKey = has2FAkey.Value,
                NewTwoFactorEnabled = twoFactorEnabled.Value
            };
            return View(model);
        }

        public async Task<IActionResult> ViewTwoFAKey()
        {
            var user = await GetCurrentUserAsync();
            var returnList = (await _userService.View2FAKeyAsync(user.Id, await _appsContainer.AccessToken())).Items;
            var model = new View2FAKeyViewModel(user)
            {
                NewTwoFAKey = returnList.Select(t => t.TwoFAKey).FirstOrDefault().ToString(),
                NewTwoFAQRUri = returnList.Select(t => t.TwoFAQRUri).FirstOrDefault().ToString()
            };
            return View(model);

        }

        public async Task<IActionResult> SetTwoFAKey()
        {
            var user = await GetCurrentUserAsync();
            var ReturnValue = (await _userService.SetTwoFAKeyAsync(user.Id, await _appsContainer.AccessToken())).Value;
            if (ReturnValue)
            {
                return RedirectToAction(nameof(ViewTwoFAKey));
            }
            else
            {
                //error page
                return View();
            }
        }

        public async Task<IActionResult> ResetTwoFAKey()
        {
            var user = await GetCurrentUserAsync();
            var ReturnValue = (await _userService.ResetTwoFAKeyAsync(user.Id, await _appsContainer.AccessToken())).Value;
            if (ReturnValue)
            {
                return RedirectToAction(nameof(ViewTwoFAKey));
            }
            else
            {
                //error page
                return View();
            }
        }
        
        public async Task<IActionResult> VerifyTwoFACode()
        {
            var user = await GetCurrentUserAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTwoFACode(VerifyTwoFACodeViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var ReturnValue = (await _userService.TwoFAVerificyCodeAsync(user.Id, await _appsContainer.AccessToken(), model.NewCode)).Value;
            if (ReturnValue)
            {
                // go to recoverycodes page
 
                return RedirectToAction(nameof(ViewTwoFAKey));
            }
            else
            {
                //error page
                return View();
            }
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

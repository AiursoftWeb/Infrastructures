using Aiursoft.Pylon;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Account.Models;
using Aiursoft.Account.Data;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Account.Models.AccountViewModels;
using Aiursoft.Pylon.Services.ToAPIServer;
using Aiursoft.Pylon.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon.Services;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Account.Controllers
{
    [AiurForceAuth]
    public class AccountController : Controller
    {
        private readonly UserManager<AccountUser> _userManager;
        private readonly SignInManager<AccountUser> _signInManager;
        private readonly ILogger _logger;
        private readonly AccountDbContext _dbContext;
        private readonly AiurSMSSender _sender;
        private readonly UserService _userService;
        private readonly StorageService _storageService;
        private readonly AppsContainer _appsContainer;

        public AccountController(
            UserManager<AccountUser> userManager,
            SignInManager<AccountUser> signInManager,
            ILoggerFactory loggerFactory,
            AccountDbContext context,
            AiurSMSSender sender,
            UserService userService,
            StorageService storageService,
            AppsContainer appsContainer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _dbContext = context;
            _sender = sender;
            _userService = userService;
            _storageService = storageService;
            _appsContainer = appsContainer;
        }

        public async Task<IActionResult> Index(bool? JustHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user)
            {
                JustHaveUpdated = JustHaveUpdated ?? false
            };
            return View(model);
        }

        [HttpPost]
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
            await _userService.ChangeProfileAsync(cuser.Id, await _appsContainer.AccessToken(), cuser.NickName, string.Empty, cuser.Bio);
            await _userManager.UpdateAsync(cuser);
            return RedirectToAction(nameof(Index), new { JustHaveUpdated = true });
        }

        [HttpGet]
        public async Task<IActionResult> Email(bool JustHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var emails = await _userService.ViewAllEmailsAsync(await _appsContainer.AccessToken(), user.Id);
            var model = new EmailViewModel(user)
            {
                Emails = emails.Items,
                PrimaryEmail = user.Email
            };
            model.JustHaveUpdated = JustHaveUpdated;
            return View(model);
        }

        [HttpPost]
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
            var result = await _userService.BindNewEmailAsync(user.Id, model.NewEmail, token);
            return RedirectToAction(nameof(Email), new { JustHaveUpdated = true });
        }

        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> SendEmail([EmailAddress]string email)
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var result = await _userService.SendConfirmationEmailAsync(token, user.Id, email);
            return Json(result);
        }
        
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> DeleteEmail([EmailAddress]string email)
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var result = await _userService.DeleteEmailAsync(user.Id, email, token);
            return Json(result);
        }

        public async Task<IActionResult> Avatar(bool JustHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var model = new AvatarViewModel(user)
            {
                JustHaveUpdated = JustHaveUpdated
            };
            return View(model);
        }

        [HttpPost]
        [FileChecker]
        public async Task<IActionResult> Avatar(AvatarViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(cuser);
                return View(model);
            }
            cuser.HeadImgUrl = await _storageService.SaveToOSS(Request.Form.Files.First(), Values.UsersIconBucketId, 365);
            await _userService.ChangeProfileAsync(cuser.Id, await _appsContainer.AccessToken(), string.Empty, cuser.HeadImgUrl, string.Empty);
            await _userManager.UpdateAsync(cuser);
            return RedirectToAction(nameof(Avatar), new { JustHaveUpdated = true });
        }

        public async Task<IActionResult> Security(bool JustHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var model = new SecurityViewModel(user)
            {
                JustHaveUpdated = JustHaveUpdated
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Security(SecurityViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(cuser);
                return View(model);
            }
            var result = await _userService.ChangePasswordAsync(cuser.Id, await _appsContainer.AccessToken(), model.OldPassword, model.NewPassword);
            if (result.Code == ErrorType.Success)
            {
                return RedirectToAction(nameof(Security), new { JustHaveUpdated = true });
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                model.ModelStateValid = false;
                model.Recover(cuser);
                return View(model);
            }
        }

        public async Task<IActionResult> Phone(bool JustHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var phone = await _userService.ViewPhoneNumberAsync(user.Id, await _appsContainer.AccessToken());
            var model = new PhoneViewModel(user)
            {
                CurrentPhoneNumber = phone.Value,
                PhoneNumberConfirmed = !string.IsNullOrEmpty(phone.Value),
                JustHaveUpdated = JustHaveUpdated
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Phone(PhoneViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(user);
                model.ModelStateValid = ModelState.IsValid;
                return View(model);
            }
            if (model.NewPhoneNumber.Length == 11)
            {
                model.NewPhoneNumber = "+86" + model.NewPhoneNumber;
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.NewPhoneNumber);
            await _sender.SendAsync(model.NewPhoneNumber, "Your security code is: " + code);
            return RedirectToAction(nameof(EnterCode), new { model.NewPhoneNumber });
        }

        public async Task<IActionResult> EnterCode(string NewPhoneNumber)
        {
            var user = await GetCurrentUserAsync();
            var model = new EnterCodeViewModel(user)
            {
                NewPhoneNumber = NewPhoneNumber
            };
            return View(model);
        }

        [HttpPost]
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
                ModelState.AddModelError("", "Your token is invalid!");
                return View(model);
            }
        }

        [HttpPost]
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

        private async Task<AccountUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}

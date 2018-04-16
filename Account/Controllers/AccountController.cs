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

        public AccountController(
            UserManager<AccountUser> userManager,
            SignInManager<AccountUser> signInManager,
            ILoggerFactory loggerFactory,
            AccountDbContext context,
            AiurSMSSender sender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _dbContext = context;
            _sender = sender;
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
            await UserService.ChangeProfileAsync(cuser.Id, await AppsContainer.AccessToken()(), cuser.NickName, string.Empty, cuser.Bio);
            await _userManager.UpdateAsync(cuser);
            return RedirectToAction(nameof(Index), new { JustHaveUpdated = true });
        }

        [HttpGet]
        public async Task<IActionResult> Email(bool JustHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var emails = await UserService.ViewAllEmailsAsync(await AppsContainer.AccessToken()(), user.Id);
            var model = new EmailViewModel(user)
            {
                Emails = emails.Items,
                PrimaryEmail = user.Email
            };
            model.JustHaveUpdated = JustHaveUpdated;
            return View(model);
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
            cuser.HeadImgUrl = await StorageService.SaveToOSS(Request.Form.Files.First(), Values.UsersIconBucketId, 365);
            await UserService.ChangeProfileAsync(cuser.Id, await AppsContainer.AccessToken()(), string.Empty, cuser.HeadImgUrl, string.Empty);
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
            var result = await UserService.ChangePasswordAsync(cuser.Id, await AppsContainer.AccessToken()(), model.OldPassword, model.NewPassword);
            if (result.code == ErrorType.Success)
            {
                return RedirectToAction(nameof(Security), new { JustHaveUpdated = true });
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.message);
                model.ModelStateValid = false;
                model.Recover(cuser);
                return View(model);
            }
        }

        public async Task<IActionResult> Phone(bool JustHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var phone = await UserService.ViewPhoneNumberAsync(user.Id, await AppsContainer.AccessToken()());
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
                var result = await UserService.SetPhoneNumberAsync(user.Id, await AppsContainer.AccessToken()(), model.NewPhoneNumber);
                if (result.code == ErrorType.Success)
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
            var result = await UserService.SetPhoneNumberAsync(user.Id, await AppsContainer.AccessToken()(), string.Empty);
            if (result.code == ErrorType.Success)
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

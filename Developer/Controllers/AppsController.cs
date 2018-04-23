using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToOSSServer;
using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models;
using Aiursoft.Developer.Models.AppsViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [AiurForceAuth]
    public class AppsController : Controller
    {
        private readonly UserManager<DeveloperUser> _userManager;
        private readonly SignInManager<DeveloperUser> _signInManager;
        private readonly ILogger _logger;
        private readonly DeveloperDbContext _dbContext;
        private readonly ServiceLocation _serviceLocation;
        private readonly StorageService _storageService;
        private readonly OSSApiService _ossApiService;
        private readonly AppsContainer _appsContainer;

        public AppsController(
        UserManager<DeveloperUser> userManager,
        SignInManager<DeveloperUser> signInManager,
        ILoggerFactory loggerFactory,
        DeveloperDbContext _context,
        ServiceLocation serviceLocation,
        StorageService storageService,
        OSSApiService ossApiService,
        AppsContainer appsContainer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AppsController>();
            _dbContext = _context;
            _serviceLocation = serviceLocation;
            _storageService = storageService;
            _ossApiService = ossApiService;
            _appsContainer = appsContainer;
        }

        public async Task<IActionResult> Index()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new IndexViewModel(cuser);
            return View(model);
        }

        public async Task<IActionResult> AllApps()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new AllAppsViewModel(cuser)
            {
                AllApps = _dbContext.Apps.Where(t => t.CreaterId == cuser.Id)
            };
            return View(model);
        }

        public async Task<IActionResult> CreateApp()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new CreateAppViewModel(cuser);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateApp(CreateAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(cuser, 1);
                return View(model);
            }
            string iconPath = string.Empty;
            if (Request.Form.Files.Count == 0 || Request.Form.Files.First().Length < 1)
            {
                iconPath = $"{_serviceLocation.CDN}/images/appdefaulticon.png";
            }
            else
            {
                iconPath = await _storageService.SaveToOSS(Request.Form.Files.First(), Values.AppsIconBucketId, 365);
            }

            var _newApp = new App(cuser.Id, model.AppName, model.AppDescription, model.AppCategory, model.AppPlatform)
            {
                CreaterId = cuser.Id,
                AppIconAddress = iconPath
            };
            _dbContext.Apps.Add(_newApp);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = _newApp.AppId });
        }

        public async Task<IActionResult> ViewApp(string id, bool JustHaveUpdated = false)
        {
            var app = await _dbContext.Apps.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            var cuser = await GetCurrentUserAsync();
            var model = await ViewAppViewModel.SelfCreateAsync(cuser, app, _dbContext);
            model.JustHaveUpdated = JustHaveUpdated;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ViewApp(ViewAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                await model.Recover(cuser, await _dbContext.Apps.FindAsync(model.AppId), _dbContext);
                return View(model);
            }
            var target = await _dbContext.Apps.FindAsync(model.AppId);
            if (target == null)
            {
                return NotFound();
            }
            else if (target.CreaterId != cuser.Id)
            {
                return new UnauthorizedResult();
            }
            target.AppName = model.AppName;
            target.AppDescription = model.AppDescription;
            target.EnableOAuth = model.EnableOAuth;
            target.ForceInputPassword = model.ForceInputPassword;
            target.ForceConfirmation = model.ForceConfirmation;
            target.DebugMode = model.DebugMode;
            target.PrivacyStatementUrl = model.PrivacyStatementUrl;
            target.LicenseUrl = model.LicenseUrl;
            target.AppDomain = model.AppDomain;
            target.ViewOpenId = model.ViewOpenId;
            target.ViewPhoneNumber = model.ViewPhoneNumber;
            target.ChangePhoneNumber = model.ChangePhoneNumber;
            target.ConfirmEmail = model.ConfirmEmail;
            target.ChangeBasicInfo = model.ChangeBasicInfo;
            target.ChangePassword = model.ChangePassword;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = target.AppId, JustHaveUpdated = true });
        }

        public async Task<IActionResult> DeleteApp(string id)
        {
            var cuser = await GetCurrentUserAsync();
            var _target = await _dbContext.Apps.FindAsync(id);
            if (_target.CreaterId != cuser.Id)
            {
                return new UnauthorizedResult();
            }
            var model = new DeleteAppViewModel(cuser)
            {
                AppId = _target.AppId,
                AppName = _target.AppName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApp(DeleteAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(cuser, 1);
                return View(model);
            }
            var target = await _dbContext.Apps.FindAsync(model.AppId);
            if (target == null)
            {
                return NotFound();
            }
            else if (target.CreaterId != cuser.Id)
            {
                return new UnauthorizedResult();
            }
            await _ossApiService.DeleteAppAsync(await _appsContainer.AccessToken(target.AppId, target.AppSecret), target.AppId);
            _dbContext.Apps.Remove(target);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(AllApps));

        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }

        [HttpPost]
        [FileChecker]
        public async Task<IActionResult> ChangeIcon(string AppId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewApp), new { id = AppId, JustHaveUpdated = true });
            }
            var appExists = await _dbContext.Apps.FindAsync(AppId);
            appExists.AppIconAddress = await _storageService.SaveToOSS(Request.Form.Files.First(), Values.AppsIconBucketId, 365);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = AppId, JustHaveUpdated = true });
        }
    }
}

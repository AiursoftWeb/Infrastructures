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
using Aiursoft.Pylon.Services.ToAPIServer;
using Microsoft.Extensions.Configuration;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Services.ToProbeServer;

namespace Aiursoft.Developer.Controllers
{
    [AiurForceAuth]
    [LimitPerMin]
    public class AppsController : Controller
    {
        private readonly DeveloperDbContext _dbContext;
        private readonly StorageService _storageService;
        private readonly AppsContainer _appsContainer;
        private readonly CoreApiService _coreApiService;
        private readonly IConfiguration _configuration;
        private readonly SitesService _siteService;

        public AppsController(
            DeveloperDbContext dbContext,
            StorageService storageService,
            AppsContainer appsContainer,
            CoreApiService coreApiService,
            IConfiguration configuration,
            SitesService siteService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
            _appsContainer = appsContainer;
            _coreApiService = coreApiService;
            _configuration = configuration;
            _siteService = siteService;
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
                AllApps = _dbContext.Apps.Where(t => t.CreatorId == cuser.Id)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateApp(CreateAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.RootRecover(cuser, 1);
                return View(model);
            }
            var newApp = new App(model.AppName, model.AppDescription, model.AppCategory, model.AppPlatform)
            {
                CreatorId = cuser.Id
            };
            // Default icon
            if (Request.Form.Files.Count == 0 || Request.Form.Files.First().Length < 1)
            {
                newApp.IconPath = $"{_configuration["AppsIconSiteName"]}/appdefaulticon.png";
            }
            else
            {
                var probeFile = await _storageService.SaveToProbe(Request.Form.Files.First(), _configuration["AppsIconSiteName"], newApp.AppId, SaveFileOptions.RandomName);
                newApp.IconPath = $"{probeFile.SiteName}/{probeFile.FilePath}";
            }
            _dbContext.Apps.Add(newApp);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = newApp.AppId });
        }

        public async Task<IActionResult> ViewApp(string id, bool justHaveUpdated = false)
        {
            var app = await _dbContext.Apps.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            var cuser = await GetCurrentUserAsync();
            var model = await ViewAppViewModel.SelfCreateAsync(cuser, app, _coreApiService, _appsContainer, _siteService);
            model.JustHaveUpdated = justHaveUpdated;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewApp(ViewAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                await model.Recover(cuser, await _dbContext.Apps.FindAsync(model.AppId), _coreApiService, _appsContainer, _siteService);
                return View(model);
            }
            var target = await _dbContext.Apps.FindAsync(model.AppId);
            if (target == null)
            {
                return NotFound();
            }
            else if (target.CreatorId != cuser.Id)
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
            //Permissions
            bool permissionAdded = false;
            target.ViewOpenId = _ChangePermission(target.ViewOpenId, model.ViewOpenId, ref permissionAdded);
            target.ViewPhoneNumber = _ChangePermission(target.ViewPhoneNumber, model.ViewPhoneNumber, ref permissionAdded);
            target.ChangePhoneNumber = _ChangePermission(target.ChangePhoneNumber, model.ChangePhoneNumber, ref permissionAdded);
            target.ConfirmEmail = _ChangePermission(target.ConfirmEmail, model.ConfirmEmail, ref permissionAdded);
            target.ChangeBasicInfo = _ChangePermission(target.ChangeBasicInfo, model.ChangeBasicInfo, ref permissionAdded);
            target.ChangePassword = _ChangePermission(target.ChangePassword, model.ChangePassword, ref permissionAdded);
            target.ChangeGrantInfo = _ChangePermission(target.ChangeGrantInfo, model.ChangeGrantInfo, ref permissionAdded);
            if (permissionAdded)
            {
                var token = await _appsContainer.AccessToken(target.AppId, target.AppSecret);
                await _coreApiService.DropGrantsAsync(token);
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = target.AppId, JustHaveUpdated = true });
        }

        private bool _ChangePermission(bool inDatabase, bool newValue, ref bool changemark)
        {
            //More permission
            if (inDatabase == false && newValue == true)
            {
                changemark = true;
                return true;
            }
            //Less permission
            else if (inDatabase == true && newValue == false)
            {
                return false;
            }
            //Not changed
            else if (inDatabase == newValue)
            {
                return newValue;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<IActionResult> DeleteApp(string id)
        {
            var cuser = await GetCurrentUserAsync();
            var _target = await _dbContext.Apps.FindAsync(id);
            if (_target.CreatorId != cuser.Id)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApp(DeleteAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.RootRecover(cuser, 1);
                return View(model);
            }
            var target = await _dbContext.Apps.FindAsync(model.AppId);
            if (target == null)
            {
                return NotFound();
            }
            else if (target.CreatorId != cuser.Id)
            {
                return new UnauthorizedResult();
            }
            try
            {
                var token = await _appsContainer.AccessToken(target.AppId, target.AppSecret);
                await _siteService.DeleteAppAsync(token, target.AppId);
            }
            catch (AiurUnexceptedResponse e)
            {
                if (e.Response.Code != ErrorType.HasDoneAlready) throw e;
            }
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeIcon(string appId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewApp), new { id = appId, JustHaveUpdated = true });
            }
            var appExists = await _dbContext.Apps.FindAsync(appId);
            var probeFile = await _storageService.SaveToProbe(Request.Form.Files.First(), _configuration["AppsIconSiteName"], appId, SaveFileOptions.RandomName);
            appExists.IconPath = $"{probeFile.SiteName}/{probeFile.FilePath}";
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = appId, JustHaveUpdated = true });
        }
    }
}

using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models.AppsViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToAPIServer;
using Aiursoft.Pylon.Services.ToProbeServer;
using Aiursoft.Pylon.Services.ToStatusServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [AiurForceAuth]
    [LimitPerMin]
    [Route("Dashboard")]
    public class AppsController : Controller
    {
        private readonly DeveloperDbContext _dbContext;
        private readonly AppsContainer _appsContainer;
        private readonly CoreApiService _coreApiService;
        private readonly SitesService _siteService;
        private readonly EventService _eventService;

        public AppsController(
            DeveloperDbContext dbContext,
            AppsContainer appsContainer,
            CoreApiService coreApiService,
            SitesService siteService,
            EventService eventService)
        {
            _dbContext = dbContext;
            _appsContainer = appsContainer;
            _coreApiService = coreApiService;
            _siteService = siteService;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new IndexViewModel(cuser);
            return View(model);
        }

        [Route("Apps")]
        public async Task<IActionResult> AllApps()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new AllAppsViewModel(cuser)
            {
                AllApps = _dbContext.Apps.Where(t => t.CreatorId == cuser.Id)
            };
            return View(model);
        }

        [Route("Apps/Create")]
        public async Task<IActionResult> CreateApp()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new CreateAppViewModel(cuser);
            return View(model);
        }

        [Route("Apps/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateApp(CreateAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.RootRecover(cuser);
                return View(model);
            }
            var newApp = new App(model.AppName, model.AppDescription, model.AppCategory, model.AppPlatform, model.IconPath)
            {
                CreatorId = cuser.Id
            };
            _dbContext.Apps.Add(newApp);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = newApp.AppId });
        }

        [Route("Apps/{id}")]
        public async Task<IActionResult> ViewApp([FromRoute]string id, bool justHaveUpdated = false)
        {
            var app = await _dbContext.Apps.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            var cuser = await GetCurrentUserAsync();
            var model = await ViewAppViewModel.SelfCreateAsync(cuser, app, _coreApiService, _appsContainer, _siteService, _eventService);
            model.JustHaveUpdated = justHaveUpdated;
            return View(model);
        }

        [Route("Apps/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewApp([FromRoute]string id, ViewAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                await model.Recover(cuser, await _dbContext.Apps.FindAsync(model.AppId), _coreApiService, _appsContainer, _siteService, _eventService);
                return View(model);
            }
            var target = await _dbContext.Apps.FindAsync(id);
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
            target.ViewAuditLog = _ChangePermission(target.ViewAuditLog, model.ViewAuditLog, ref permissionAdded);
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
            if (inDatabase == false && newValue)
            {
                changemark = true;
                return true;
            }
            //Less permission
            else if (inDatabase && newValue == false)
            {
                return false;
            }
            //Not changed
            else
            {
                return newValue;
            }
        }

        [Route("Apps/{id}/Delete")]
        public async Task<IActionResult> DeleteApp([FromRoute]string id)
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

        [Route("Apps/{id}/Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApp([FromRoute]string id, DeleteAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.RootRecover(cuser);
                return View(model);
            }
            var target = await _dbContext.Apps.FindAsync(id);
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
                if (e.Response.Code != ErrorType.HasDoneAlready) throw;
            }
            _dbContext.Apps.Remove(target);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(AllApps));

        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }

        [Route("Apps/{appId}/ChangeIcon")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeIcon([FromRoute]string appId, string iconFile)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewApp), new { id = appId, JustHaveUpdated = true });
            }
            var appExists = await _dbContext.Apps.FindAsync(appId);
            appExists.IconPath = iconFile;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = appId, JustHaveUpdated = true });
        }
    }
}

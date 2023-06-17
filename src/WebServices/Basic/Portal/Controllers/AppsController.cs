using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Portal.Data;
using Aiursoft.Portal.Models;
using Aiursoft.Portal.Models.AppsViewModels;
using Aiursoft.Directory.SDK.Services.ToDirectoryServer;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Identity.Attributes;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.Stargate.SDK.Services.ToStargateServer;
using Aiursoft.Warpgate.SDK.Services.ToWarpgateServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Portal.Controllers;

[AiurForceAuth]
[LimitPerMin]
[Route("Dashboard")]
public class AppsController : Controller
{
    private readonly DirectoryAppTokenService _directoryAppTokenService;
    private readonly ChannelService _channelService;
    private readonly AppsService _coreApiService;
    private readonly PortalDbContext _dbContext;
    private readonly ObserverService _eventService;
    private readonly RecordsService _recordsService;
    private readonly SitesService _siteService;

    public AppsController(
        PortalDbContext dbContext,
        DirectoryAppTokenService directoryAppTokenService,
        AppsService coreApiService,
        SitesService siteService,
        ObserverService eventService,
        ChannelService channelService,
        RecordsService recordsService)
    {
        _dbContext = dbContext;
        _directoryAppTokenService = directoryAppTokenService;
        _coreApiService = coreApiService;
        _siteService = siteService;
        _eventService = eventService;
        _channelService = channelService;
        _recordsService = recordsService;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(AllApps));
    }

    [Route("Apps")]
    public async Task<IActionResult> AllApps()
    {
        var currentUser = await GetCurrentUserAsync();
        var allMyApps = await _coreApiService.GetMyApps(
            await _directoryAppTokenService.GetAccessTokenAsync(), currentUser.Id);
        var model = new AllAppsViewModel(currentUser, allMyApps);
        return View(model);
    }

    [Route("Apps/Create")]
    public async Task<IActionResult> CreateApp()
    {
        var currentUser = await GetCurrentUserAsync();
        var allMyApps = await _coreApiService.GetMyApps(
            await _directoryAppTokenService.GetAccessTokenAsync(), currentUser.Id);
        var model = new CreateAppViewModel(currentUser, allMyApps);
        return View(model);
    }

    [Route("Apps/Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateApp(CreateAppViewModel model)
    {
        var currentUser = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            var allMyApps = await _coreApiService.GetMyApps(
                await _directoryAppTokenService.GetAccessTokenAsync(), currentUser.Id);
            model.RootRecover(currentUser, allMyApps);
            return View(model);
        }

        var newApp = new DeveloperApp(model.AppName, model.AppDescription, model.IconPath)
        {
            CreatorId = currentUser.Id
        };
        await _dbContext.Apps.AddAsync(newApp);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(ViewApp), new { id = newApp.AppId });
    }

    [Route("Apps/{id}")]
    public async Task<IActionResult> ViewApp([FromRoute] string id, int page = 1, bool justHaveUpdated = false)
    {
        var app = await _dbContext.Apps.FindAsync(id);
        if (app == null)
        {
            return NotFound();
        }

        var currentUser = await GetCurrentUserAsync();
        var model = await ViewAppViewModel.SelfCreateAsync(currentUser, app, _coreApiService, _directoryAppTokenService,
            _siteService, _eventService, _channelService, _recordsService, page);
        model.JustHaveUpdated = justHaveUpdated;
        return View(model);
    }

    [Route("Apps/{id}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ViewApp([FromRoute] string id, ViewAppViewModel model, int page = 1)
    {
        var currentUser = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            await model.Recover(currentUser, await _dbContext.Apps.FindAsync(model.AppId), _coreApiService,
                _directoryAppTokenService, _siteService, _eventService, _channelService, _recordsService, page);
            return View(model);
        }

        var target = await _dbContext.Apps.FindAsync(id);
        if (target == null)
        {
            return NotFound();
        }

        if (target.CreatorId != currentUser.Id)
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
        // Permissions
        var permissionAdded = false;
        target.ViewOpenId = _ChangePermission(target.ViewOpenId, model.ViewOpenId, ref permissionAdded);
        target.ViewPhoneNumber = _ChangePermission(target.ViewPhoneNumber, model.ViewPhoneNumber, ref permissionAdded);
        target.ChangePhoneNumber =
            _ChangePermission(target.ChangePhoneNumber, model.ChangePhoneNumber, ref permissionAdded);
        target.ConfirmEmail = _ChangePermission(target.ConfirmEmail, model.ConfirmEmail, ref permissionAdded);
        target.ChangeBasicInfo = _ChangePermission(target.ChangeBasicInfo, model.ChangeBasicInfo, ref permissionAdded);
        target.ChangePassword = _ChangePermission(target.ChangePassword, model.ChangePassword, ref permissionAdded);
        target.ChangeGrantInfo = _ChangePermission(target.ChangeGrantInfo, model.ChangeGrantInfo, ref permissionAdded);
        target.ViewAuditLog = _ChangePermission(target.ViewAuditLog, model.ViewAuditLog, ref permissionAdded);
        target.ManageSocialAccount =
            _ChangePermission(target.ManageSocialAccount, model.ManageSocialAccount, ref permissionAdded);
        if (permissionAdded)
        {
            var token = await _directoryAppTokenService.GetAccessTokenWithAppInfoAsync(target.AppId, target.AppSecret);
            await _coreApiService.DropGrantsAsync(token);
        }

        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(ViewApp), new { id = target.AppId, JustHaveUpdated = true });
    }

    private bool _ChangePermission(bool inDatabase, bool newValue, ref bool changeMark)
    {
        // More permission
        if (inDatabase == false && newValue)
        {
            changeMark = true;
            return true;
        }
        // Less permission

        if (inDatabase && newValue == false)
        {
            return false;
        }

        // Not changed
        return newValue;
    }

    [Route("Apps/{id}/Delete")]
    public async Task<IActionResult> DeleteApp([FromRoute] string id)
    {
        var currentUser = await GetCurrentUserAsync();
        var target = await _dbContext.Apps.FindAsync(id);
        if (target == null)
        {
            return NotFound();
        }

        if (target.CreatorId != currentUser.Id)
        {
            return new UnauthorizedResult();
        }

        var model = new DeleteAppViewModel(currentUser)
        {
            AppId = target.AppId,
            AppName = target.AppName
        };
        return View(model);
    }

    [Route("Apps/{id}/Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteApp([FromRoute] string id, DeleteAppViewModel model)
    {
        var currentUser = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            model.RootRecover(currentUser);
            return View(model);
        }

        var target = await _dbContext.Apps.FindAsync(id);
        if (target == null)
        {
            return NotFound();
        }

        if (target.CreatorId != currentUser.Id)
        {
            return new UnauthorizedResult();
        }

        try
        {
            var token = await _directoryAppTokenService.GetAccessTokenWithAppInfoAsync(target.AppId, target.AppSecret);
            await _siteService.DeleteAppAsync(token, target.AppId);
            await _recordsService.DeleteAppAsync(token, target.AppId);
            await _eventService.DeleteAppAsync(token, target.AppId);
        }
        catch (AiurUnexpectedResponse e) when (e.Response.Code == ErrorType.HasSuccessAlready)
        {
        }

        _dbContext.Apps.Remove(target);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(AllApps));
    }

    [Route("Apps/{appId}/ChangeIcon")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeIcon([FromRoute] string appId, string iconFile)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(ViewApp), new { id = appId, JustHaveUpdated = true });
        }

        var appExists = await _dbContext.Apps.FindAsync(appId);
        if (appExists != null)
        {
            appExists.IconPath = iconFile;
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction(nameof(ViewApp), new { id = appId, JustHaveUpdated = true });
    }
    
    private async Task<PortalUser> GetCurrentUserAsync()
    {
        return await _dbContext.Users.Include(t => t.MyApps)
            .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
    }
}
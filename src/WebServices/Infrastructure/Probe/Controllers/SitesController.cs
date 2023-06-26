using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Probe.Repositories;
using Aiursoft.Probe.SDK.Models.SitesAddressModels;
using Aiursoft.Probe.SDK.Models.SitesViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Probe.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class SitesController : ControllerBase
{
    private readonly AppRepo _appRepo;
    private readonly FolderRepo _folderRepo;
    private readonly SiteRepo _siteRepo;

    public SitesController(
        AppRepo appRepo,
        FolderRepo folderRepo,
        SiteRepo siteRepo)
    {
        _appRepo = appRepo;
        _folderRepo = folderRepo;
        _siteRepo = siteRepo;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewSite(CreateNewSiteAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var createdSite =
            await _siteRepo.CreateSite(model.NewSiteName, model.OpenToUpload, model.OpenToDownload, appid);
        return this.Protocol(Code.JobDone,
            $"Successfully created your new site: '{createdSite.SiteName}' at {createdSite.CreationTime}.");
    }

    [Produces(typeof(ViewMySitesViewModel))]
    public async Task<IActionResult> ViewMySites(ViewMySitesAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var sites = await _siteRepo.GetAllSitesUnderApp(appid);
        var viewModel = new ViewMySitesViewModel
        {
            AppId = appid,
            Sites = sites,
            Code = Code.ResultShown,
            Message = "Successfully get all your sites!"
        };
        return this.Protocol(viewModel);
    }

    [Produces(typeof(ViewSiteDetailViewModel))]
    public async Task<IActionResult> ViewSiteDetail(ViewSiteDetailAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var site = await _siteRepo.GetSiteByNameUnderApp(model.SiteName, appid);
        var viewModel = new ViewSiteDetailViewModel
        {
            AppId = appid,
            Site = site,
            Size = await _folderRepo.GetFolderSize(site.RootFolderId),
            Code = Code.ResultShown,
            Message = "Successfully get your site!"
        };
        return this.Protocol(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSiteInfo(UpdateSiteInfoAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var site = await _siteRepo.GetSiteByNameUnderApp(model.OldSiteName, appid);
        // Conflict = Name changed, and new name already exists.
        var conflict = model.NewSiteName.ToLower() != model.OldSiteName.ToLower() &&
                       await _siteRepo.GetSiteByName(model.NewSiteName) != null;
        if (conflict)
        {
            return this.Protocol(Code.Conflict,
                $"There is already a site with name: '{model.NewSiteName}'. Please try another new name.");
        }

        site.SiteName = model.NewSiteName;
        site.OpenToDownload = model.OpenToDownload;
        site.OpenToUpload = model.OpenToUpload;
        await _siteRepo.UpdateSite(site);
        return this.Protocol(Code.JobDone, "Successfully updated your site!");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSite(DeleteSiteAddressModel model)
    {
        var appid = await _appRepo.GetAppId(model.AccessToken);
        var site = await _siteRepo.GetSiteByNameUnderApp(model.SiteName, appid);
        await _siteRepo.DeleteSite(site);
        return this.Protocol(Code.JobDone, "Successfully deleted your site!");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteApp(DeleteAppAddressModel model)
    {
        var app = await _appRepo.GetApp(model.AccessToken);
        if (app.AppId != model.AppId)
        {
            return this.Protocol(Code.Unauthorized,
                "The app you try to delete is not the access token you granted!");
        }

        await _appRepo.DeleteApp(app);
        return this.Protocol(Code.JobDone, $"That app with ID: {app.AppId} was successfully deleted!");
    }
}
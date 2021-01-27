using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Repositories;
using Aiursoft.Probe.SDK.Models.SitesAddressModels;
using Aiursoft.Probe.SDK.Models.SitesViewModels;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [LimitPerMin]
    [APIExpHandler]
    [APIModelStateChecker]
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
            var createdSite = await _siteRepo.CreateSite(model.NewSiteName, model.OpenToUpload, model.OpenToDownload, appid);
            return this.Protocol(ErrorType.Success, $"Successfully created your new site: '{createdSite.SiteName}' at {createdSite.CreationTime}.");
        }

        [APIProduces(typeof(ViewMySitesViewModel))]
        public async Task<IActionResult> ViewMySites(ViewMySitesAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);
            var sites = await _siteRepo.GetAllSitesUnderApp(appid);
            var viewModel = new ViewMySitesViewModel
            {
                AppId = appid,
                Sites = sites,
                Code = ErrorType.Success,
                Message = "Successfully get all your sites!"
            };
            return this.Protocol(viewModel);
        }

        [APIProduces(typeof(ViewSiteDetailViewModel))]
        public async Task<IActionResult> ViewSiteDetail(ViewSiteDetailAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);
            var site = await _siteRepo.GetSiteByNameUnderApp(model.SiteName, appid);
            var viewModel = new ViewSiteDetailViewModel
            {
                AppId = appid,
                Site = site,
                Size = await _folderRepo.GetFolderSize(site.RootFolderId),
                Code = ErrorType.Success,
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
                return this.Protocol(ErrorType.Conflict, $"There is already a site with name: '{model.NewSiteName}'. Please try another new name.");
            }
            site.SiteName = model.NewSiteName;
            site.OpenToDownload = model.OpenToDownload;
            site.OpenToUpload = model.OpenToUpload;
            await _siteRepo.UpdateSite(site);
            return this.Protocol(ErrorType.Success, "Successfully updated your site!");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSite(DeleteSiteAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);
            var site = await _siteRepo.GetSiteByNameUnderApp(model.SiteName, appid);
            await _siteRepo.DeleteSite(site);
            return this.Protocol(ErrorType.Success, "Successfully deleted your site!");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApp(DeleteAppAddressModel model)
        {
            var app = await _appRepo.GetApp(model.AccessToken);
            if (app.AppId != model.AppId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The app you try to delete is not the access token you granted!");
            }
            await _appRepo.DeleteApp(app);
            return this.Protocol(ErrorType.HasSuccessAlready, "That app do not exists in our database.");
        }
    }
}

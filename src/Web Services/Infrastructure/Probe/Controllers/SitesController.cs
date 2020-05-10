using Aiursoft.DBTools;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.Repositories;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.SitesAddressModels;
using Aiursoft.Probe.SDK.Models.SitesViewModels;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class SitesController : Controller
    {
        private readonly ProbeDbContext _dbContext;
        private readonly AppRepo _appRepo;
        private readonly FolderRepo _folderRepo;

        public SitesController(
            ProbeDbContext dbContext,
            AppRepo appRepo,
            FolderRepo folderRepo)
        {
            _dbContext = dbContext;
            _appRepo = appRepo;
            _folderRepo = folderRepo;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewSite(CreateNewSiteAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);
            var conflict = await _dbContext.Sites
                .AnyAsync(t => t.SiteName.ToLower().Trim() == model.NewSiteName.ToLower().Trim());
            if (conflict)
            {
                return this.Protocol(ErrorType.NotEnoughResources, $"There is already a site with name: '{model.NewSiteName}'. Please try another new name.");
            }
            var newRootFolder = new Folder
            {
                FolderName = "blob"
            };
            _dbContext.Folders.Add(newRootFolder);
            await _dbContext.SaveChangesAsync();
            var site = new Site
            {
                AppId = appid,
                SiteName = model.NewSiteName.ToLower(),
                RootFolderId = newRootFolder.Id,
                OpenToUpload = model.OpenToUpload,
                OpenToDownload = model.OpenToDownload
            };
            _dbContext.Sites.Add(site);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"Successfully created your new site: '{site.SiteName}'.");
        }

        [APIProduces(typeof(ViewMySitesViewModel))]
        public async Task<IActionResult> ViewMySites(ViewMySitesAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);
            var sites = await _dbContext
                .Sites
                .Where(t => t.AppId == appid)
                .Include(t => t.Root)
                .ToListAsync();
            var viewModel = new ViewMySitesViewModel
            {
                AppId = appid,
                Sites = sites,
                Code = ErrorType.Success,
                Message = "Successfully get your buckets!"
            };
            return Json(viewModel);
        }

        [APIProduces(typeof(ViewSiteDetailViewModel))]
        public async Task<IActionResult> ViewSiteDetail(ViewSiteDetailAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);

            var site = await _dbContext
                .Sites
                .Where(t => t.AppId == appid)
                .Include(t => t.Root)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == model.SiteName.ToLower());
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Could not find your site with name: {model.SiteName}");
            }
            var viewModel = new ViewSiteDetailViewModel
            {
                AppId = appid,
                Site = site,
                Size = await _folderRepo.GetFolderSize(site.RootFolderId),
                Code = ErrorType.Success,
                Message = "Successfully get your buckets!"
            };
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSiteInfo(UpdateSiteInfoAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .SingleOrDefaultAsync(t => t.SiteName == model.OldSiteName);
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Could not find a site with name: '{model.OldSiteName}'");
            }
            if (site.AppId != appid)
            {
                return this.Protocol(ErrorType.Unauthorized, $"The site you tried to update is not your app's site.");
            }
            var conflict = await _dbContext.Sites
                .Where(t => t.Id != site.Id)
                .AnyAsync(t => t.SiteName.ToLower().Trim() == model.NewSiteName.ToLower().Trim());
            if (conflict)
            {
                return this.Protocol(ErrorType.NotEnoughResources, $"There is already a site with name: '{model.NewSiteName}'. Please try another new name.");
            }
            site.SiteName = model.NewSiteName;
            site.OpenToDownload = model.OpenToDownload;
            site.OpenToUpload = model.OpenToUpload;
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully updated your site!");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSite(DeleteSiteAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);
            var site = await _dbContext
                .Sites
                .SingleOrDefaultAsync(t => t.SiteName == model.SiteName);
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Could not find a site with name: '{model.SiteName}'");
            }
            if (site.AppId != appid)
            {
                return this.Protocol(ErrorType.Unauthorized, $"The site you tried to delete is not your app's site.");
            }
            await _folderRepo.DeleteFolder(site.RootFolderId);
            _dbContext.Sites.Remove(site);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully deleted your site!");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApp(DeleteAppAddressModel model)
        {
            var appid = await _appRepo.GetAppId(model.AccessToken);
            if (appid != model.AppId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The app you try to delete is not the access token you granted!");
            }
            var target = await _dbContext
                .Apps
                .Include(t => t.Sites)
                .SingleOrDefaultAsync(t => t.AppId == appid);
            if (target != null)
            {
                _dbContext.Folders.Delete(t => target.Sites.Select(p => p.RootFolderId).Contains(t.Id));
                _dbContext.Sites.Delete(t => t.AppId == appid);
                _dbContext.Apps.Remove(target);
                await _dbContext.SaveChangesAsync();
                return this.Protocol(ErrorType.Success, "Successfully deleted that app and all sites.");
            }
            return this.Protocol(ErrorType.HasDoneAlready, "That app do not exists in our database.");
        }
    }
}

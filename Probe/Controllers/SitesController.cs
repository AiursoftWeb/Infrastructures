using Aiursoft.Probe.Data;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Probe;
using Aiursoft.Pylon.Models.Probe.SitesAddressModels;
using Aiursoft.Pylon.Models.Probe.SitesViewModels;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class SitesController : Controller
    {
        private readonly ProbeDbContext _dbContext;
        private readonly ACTokenManager _tokenManager;

        public SitesController(
            ProbeDbContext dbContext,
            ACTokenManager tokenManager)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewSite(CreateNewSiteAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new ProbeApp
                {
                    AppId = appid
                };
                _dbContext.Apps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }

            var conflict = await _dbContext.Sites
                .AnyAsync(t => t.SiteName.ToLower().Trim() == model.NewSiteName.ToLower().Trim());
            if (conflict)
            {
                return this.Protocol(ErrorType.NotEnoughResources, $"There is already a site named: {model.NewSiteName}. Please try another new name.");
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
                SiteName = model.NewSiteName,
                FolderId = newRootFolder.Id
            };
            _dbContext.Sites.Add(site);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"Successfully created your new site: {site.SiteName}.");
        }

        public async Task<IActionResult> ViewMySites(ViewMySitesAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new ProbeApp
                {
                    AppId = appid
                };
                _dbContext.Apps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }

            var sites = await _dbContext
                .Sites
                .Where(t => t.AppId == appid)
                .ToListAsync();
            var viewModel = new ViewMySitesViewModel
            {
                AppId = appLocal.AppId,
                Sites = sites,
                Code = ErrorType.Success,
                Message = "Successfully get your buckets!"
            };
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSite(DeleteSiteAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var site = await _dbContext
                .Sites
                .SingleOrDefaultAsync(t => t.SiteName == model.SiteName);
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Could not find a site named: {model.SiteName}");
            }
            if (site.AppId != appid)
            {
                return this.Protocol(ErrorType.Unauthorized, $"The site you tried to delete is not your app's site.");
            }
            _dbContext.Sites.Remove(site);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully deleted your site!");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApp(DeleteAppAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            if (appid != model.AppId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The app you try to delete is not the access token you granted!");
            }
            var target = await _dbContext.Apps.FindAsync(appid);
            if (target != null)
            {
                _dbContext.Folders.Delete(t => target.Sites.Select(p => p.FolderId).Contains(t.Id));
                _dbContext.Sites.Delete(t => t.AppId == appid);
                _dbContext.Apps.Remove(target);
                await _dbContext.SaveChangesAsync();
                return this.Protocol(ErrorType.Success, "Successfully deleted that app and all sites.");
            }
            return this.Protocol(ErrorType.HasDoneAlready, "That app do not exists in our database.");
        }
    }
}

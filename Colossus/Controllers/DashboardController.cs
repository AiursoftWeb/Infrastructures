using Aiursoft.Archon.SDK.Services;
using Aiursoft.Colossus.Models;
using Aiursoft.Colossus.Models.DashboardViewModels;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Probe.SDK.Services.ToProbeServer;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Controllers
{
    [LimitPerMin]
    [AiurForceAuth]
    [Route("Dashboard")]
    public class DashboardController : Controller
    {
        private readonly SitesService _sitesService;
        private readonly AppsContainer _appsContainer;
        private readonly UserManager<ColossusUser> _userManager;
        private readonly FoldersService _foldersService;
        private readonly FilesService _filesService;

        private Task<string> _accesstoken => _appsContainer.AccessToken();

        public DashboardController(
            SitesService sitesService,
            AppsContainer appsContainer,
            UserManager<ColossusUser> userManager,
            FoldersService foldersService,
            FilesService filesService)
        {
            _sitesService = sitesService;
            _appsContainer = appsContainer;
            _userManager = userManager;
            _foldersService = foldersService;
            _filesService = filesService;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var sites = await _sitesService.ViewMySitesAsync(await _accesstoken);
            if (string.IsNullOrEmpty(user.SiteName) || !sites.Sites.Any(t => t.SiteName == user.SiteName))
            {
                return RedirectToAction(nameof(CreateSite));
            }
            var model = new IndexViewModel(user)
            {
                SiteName = user.SiteName
            };
            return View(model);
        }

        [Route("CreateSite")]
        public async Task<IActionResult> CreateSite()
        {
            var user = await GetCurrentUserAsync();
            var sites = await _sitesService.ViewMySitesAsync(await _accesstoken);
            if (!string.IsNullOrEmpty(user.SiteName) && sites.Sites.Any(t => t.SiteName == user.SiteName))
            {
                return RedirectToAction(nameof(Index));
            }
            var model = new CreateSiteViewModel(user)
            {

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateSite")]
        public async Task<IActionResult> CreateSite(CreateSiteViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            try
            {
                await _sitesService.CreateNewSiteAsync(await _accesstoken, model.SiteName, model.OpenToUpload, model.OpenToDownload);
                user.SiteName = model.SiteName;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
        }

        [Route("ViewFiles/{**path}")]
        public async Task<IActionResult> ViewFiles(string path, bool justHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            if (string.IsNullOrWhiteSpace(user.SiteName))
            {
                return RedirectToAction(nameof(CreateSite));
            }
            try
            {
                var data = await _foldersService.ViewContentAsync(await _accesstoken, user.SiteName, path);
                var model = new ViewFilesViewModel(user)
                {
                    Folder = data.Value,
                    Path = path,
                    SiteName = user.SiteName,
                    JustHaveUpdated = justHaveUpdated
                };
                return View(model);
            }
            catch (AiurUnexceptedResponse e) when (e.Code == ErrorType.NotFound)
            {
                return NotFound();
            }
        }

        [Route("NewFolder/{**path}")]
        public async Task<IActionResult> NewFolder(string path)
        {
            var user = await GetCurrentUserAsync();
            var model = new NewFolderViewModel(user)
            {
                Path = path
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("NewFolder/{**path}")]
        public async Task<IActionResult> NewFolder(NewFolderViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            try
            {
                await _foldersService.CreateNewFolderAsync(await _accesstoken, user.SiteName, model.Path, model.NewFolderName, false);
                return RedirectToAction(nameof(ViewFiles), new { path = model.Path });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
        }

        [Route("DeleteFolder/{**path}")]
        public async Task<IActionResult> DeleteFolder([FromRoute]string path)
        {
            var user = await GetCurrentUserAsync();
            var model = new DeleteFolderViewModel(user)
            {
                Path = path
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteFolder/{**path}")]
        public async Task<IActionResult> DeleteFolder(DeleteFolderViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            try
            {
                await _foldersService.DeleteFolderAsync(await _accesstoken, user.SiteName, model.Path);
                return RedirectToAction(nameof(ViewFiles), new { path = model.Path.DetachPath() });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
        }

        [Route("DeleteFile/{**path}")]
        public async Task<IActionResult> DeleteFile([FromRoute]string path)
        {
            var user = await GetCurrentUserAsync();
            var model = new DeleteFileViewModel(user)
            {
                Path = path
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteFile/{**path}")]
        public async Task<IActionResult> DeleteFile(DeleteFileViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            try
            {
                await _filesService.DeleteFileAsync(await _accesstoken, user.SiteName, model.Path);
                return RedirectToAction(nameof(ViewFiles), new { path = model.Path.DetachPath() });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
        }

        [Route("Delete")]
        public async Task<IActionResult> Delete()
        {
            var user = await GetCurrentUserAsync();
            var model = new DeleteViewModel(user)
            {
                SiteName = user.SiteName
            };
            return View(model);
        }

        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            try
            {
                await _sitesService.DeleteSiteAsync(await _accesstoken, user.SiteName);
                user.SiteName = string.Empty;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(CreateSite));
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
        }

        [Route("Settings")]
        public async Task<IActionResult> Settings(bool justHaveUpdated)
        {
            var user = await GetCurrentUserAsync();
            var sites = await _sitesService.ViewMySitesAsync(await _accesstoken);
            var hasASite = !string.IsNullOrEmpty(user.SiteName) && sites.Sites.Any(t => t.SiteName == user.SiteName);
            if (hasASite)
            {
                var siteDetail = await _sitesService.ViewSiteDetailAsync(await _accesstoken, user.SiteName);
                var model = new SettingsViewModel(user)
                {
                    SiteSize = siteDetail.Size,
                    HasASite = true,
                    JustHaveUpdated = justHaveUpdated,
                    NewSiteName = siteDetail.Site.SiteName,
                    OldSiteName = siteDetail.Site.SiteName,
                    OpenToDownload = siteDetail.Site.OpenToDownload,
                    OpenToUpload = siteDetail.Site.OpenToUpload
                };
                return View(model);
            }
            else
            {
                var model = new SettingsViewModel(user)
                {
                    HasASite = false,
                    JustHaveUpdated = justHaveUpdated
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Settings")]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.AccessToken();
                await _sitesService.UpdateSiteInfoAsync(token, model.OldSiteName, model.NewSiteName, model.OpenToUpload, model.OpenToDownload);
                user.SiteName = model.NewSiteName;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(DashboardController.Settings), "Dashboard", new { JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                model.NewSiteName = model.OldSiteName;
                return View(model);
            }
        }

        private async Task<ColossusUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}

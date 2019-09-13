using Aiursoft.Colossus.Models;
using Aiursoft.Colossus.Models.DashboardViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToProbeServer;
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
        private readonly StorageService _storageService;
        private readonly FilesService _filesService;

        private Task<string> accesstoken => _appsContainer.AccessToken();

        public DashboardController(
            SitesService sitesService,
            AppsContainer appsContainer,
            UserManager<ColossusUser> userManager,
            FoldersService foldersService,
            StorageService storageService,
            FilesService filesService)
        {
            _sitesService = sitesService;
            _appsContainer = appsContainer;
            _userManager = userManager;
            _foldersService = foldersService;
            _storageService = storageService;
            _filesService = filesService;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var sites = await _sitesService.ViewMySitesAsync(await accesstoken);
            if (string.IsNullOrEmpty(user.SiteName) || !sites.Sites.Any(t => t.SiteName == user.SiteName))
            {
                return RedirectToAction(nameof(CreateSite));
            }
            var model = new IndexViewModel(user)
            {

            };
            return View(model);
        }

        [Route("CreateSite")]
        public async Task<IActionResult> CreateSite()
        {
            var user = await GetCurrentUserAsync();
            var sites = await _sitesService.ViewMySitesAsync(await accesstoken);
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
                var token = await _appsContainer.AccessToken();
                await _sitesService.CreateNewSiteAsync(token, model.SiteName);
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
        public async Task<IActionResult> ViewFiles(string path)
        {
            var user = await GetCurrentUserAsync();
            try
            {
                var token = await _appsContainer.AccessToken();
                var data = await _foldersService.ViewContentAsync(token, user.SiteName, path);
                var model = new ViewFilesViewModel(user)
                {
                    Folder = data.Value,
                    Path = path
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
                var token = await _appsContainer.AccessToken();
                await _foldersService.CreateNewFolderAsync(token, user.SiteName, model.Path, model.NewFolderName, false);
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

        [Route("NewFile/{**path}")]
        public async Task<IActionResult> NewFile(string path)
        {
            var user = await GetCurrentUserAsync();
            var model = new NewFileViewModel(user)
            {
                Path = path
            };
            return View(model);
        }

        [HttpPost]
        [FileChecker]
        [Route("NewFile/{**path}")]
        public async Task<IActionResult> NewFile(NewFileViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var file = Request.Form.Files.First();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            string accessToken = await _appsContainer.AccessToken();
            await _storageService.SaveToProbe(file, user.SiteName, model.Path, SaveFileOptions.SourceName, accessToken);
            return RedirectToAction(nameof(ViewFiles), new { path = model.Path });
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
                var token = await _appsContainer.AccessToken();
                await _foldersService.DeleteFolderAsync(token, user.SiteName, model.Path);
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
                var token = await _appsContainer.AccessToken();
                await _filesService.DeleteFileAsync(token, user.SiteName, model.Path);
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
                var token = await _appsContainer.AccessToken();
                await _sitesService.DeleteSiteAsync(token, user.SiteName);
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

        private async Task<ColossusUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}

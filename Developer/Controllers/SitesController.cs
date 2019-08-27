using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models.SitesViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.Probe;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToProbeServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [AiurForceAuth]
    [LimitPerMin]
    [Route("Dashboard")]
    public class SitesController : Controller
    {
        public DeveloperDbContext _dbContext;
        private readonly AppsContainer _appsContainer;
        private readonly SitesService _sitesService;
        private readonly FoldersService _foldersService;
        private readonly StorageService _storageService;

        public SitesController(
            DeveloperDbContext dbContext,
            AppsContainer appsContainer,
            SitesService sitesService,
            FoldersService foldersService,
            StorageService storageService)
        {
            _dbContext = dbContext;
            _appsContainer = appsContainer;
            _sitesService = sitesService;
            _foldersService = foldersService;
            _storageService = storageService;
        }

        [Route("Sites")]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

        [Route("Apps/{id}/CreateSite")]
        public async Task<IActionResult> CreateSite([FromRoute]string id)// app id
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            var model = new CreateSiteViewModel(user)
            {
                AppId = id,
                AppName = app.AppName
            };
            return View(model);
        }

        [HttpPost]
        [Route("Apps/{id}/CreateSite")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSite([FromRoute]string id, CreateSiteViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            var app = await _dbContext.Apps.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                await _sitesService.CreateNewSiteAsync(token, model.SiteName);
                return RedirectToAction(nameof(AppsController.ViewApp), "Apps", new { id = app.AppId, JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
        }

        [Route("Apps/{appId}/Sites/{siteName}/ViewFiles/{**path}")]
        public async Task<IActionResult> ViewFiles(string appId, string siteName, string path) // siteName
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                var data = await _foldersService.ViewContentAsync(token, siteName, path);
                var model = new ViewFilesViewModel(user)
                {
                    Folder = data.Value,
                    AppId = appId,
                    SiteName = siteName,
                    AppName = app.AppName,
                    Path = path
                };
                return View(model);
            }
            catch (AiurUnexceptedResponse e) when (e.Code == ErrorType.NotFound)
            {
                return NotFound();
            }
        }

        [Route("Apps/{appId}/Sites/{siteName}/NewFolder/{**path}")]
        public async Task<IActionResult> NewFolder(string appId, string siteName, string path)
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            var model = new NewFolderViewModel(user)
            {
                AppId = appId,
                SiteName = siteName,
                Path = path,
                AppName = app.AppName
            };
            return View(model);
        }

        [HttpPost]
        [Route("Apps/{appId}/Sites/{siteName}/NewFolder/{**path}")]
        public async Task<IActionResult> NewFolder(NewFolderViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(model.AppId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                var data = await _foldersService.CreateNewFolderAsync(token, model.SiteName, model.Path, model.NewFolderName, false);
                return RedirectToAction(nameof(ViewFiles), new { appId = model.AppId, siteName = model.SiteName, path = model.Path });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
        }

        [Route("Apps/{appId}/Sites/{siteName}/NewFile/{**path}")]
        public async Task<IActionResult> NewFile(string appId, string siteName, string path)
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            var model = new NewFileViewModel(user)
            {
                AppId = appId,
                SiteName = siteName,
                Path = path,
                AppName = app.AppName
            };
            return View(model);
        }

        [HttpPost]
        [FileChecker]
        [Route("Apps/{appId}/Sites/{siteName}/NewFile/{**path}")]
        public async Task<IActionResult> NewFile(NewFileViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var file = Request.Form.Files.First();
            var app = await _dbContext.Apps.FindAsync(model.AppId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
            string accessToken = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
            await _storageService.SaveToProbe(file, model.SiteName, model.Path, SaveFileOptions.SourceName, accessToken);
            return RedirectToAction(nameof(ViewFiles), new { appId = model.AppId, siteName = model.SiteName, path = model.Path });
        }

        [Route("Apps/{appId}/Sites/{siteName}/DeleteFolder/{**path}")]
        public async Task<IActionResult> DeleteFolder([FromRoute]string appId, [FromRoute]string siteName, [FromRoute]string path)
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            var model = new DeleteFolderViewModel(user)
            {
                AppId = appId,
                SiteName = siteName,
                Path = path,
                AppName = app.AppName
            };
            return View(model);
        }

        [HttpPost]
        [Route("Apps/{appId}/Sites/{siteName}/DeleteFolder/{**path}")]
        public async Task<IActionResult> DeleteFolder(DeleteFolderViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(model.AppId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                await _foldersService.DeleteFolderAsync(token, model.SiteName, model.Path);
                return RedirectToAction(nameof(ViewFiles), new { appId = model.AppId, siteName = model.SiteName, path = model.Path.DetachPath() });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
        }

        [Route("Apps/{appId}/Sites/{siteName}/Delete")]
        public async Task<IActionResult> Delete(string appId, string siteName)
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            var model = new DeleteViewModel(user)
            {
                AppId = appId,
                SiteName = siteName,
                AppName = app.AppName
            };
            return View(model);
        }

        [Route("Apps/{appId}/Sites/{siteName}/Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(model.AppId);
            if (app == null)
            {
                return NotFound();
            }
            if (app.CreatorId != user.Id)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                await _sitesService.DeleteSiteAsync(token, model.SiteName);
                return RedirectToAction(nameof(AppsController.ViewApp), "Apps", new { id = app.AppId, JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}

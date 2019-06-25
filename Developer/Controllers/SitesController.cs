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
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [AiurForceAuth]
    [Route("Sites")]
    [LimitPerMin]
    public class SitesController : Controller
    {
        public DeveloperDbContext _dbContext;
        private readonly AppsContainer _appsContainer;
        private readonly SitesService _sitesService;
        private readonly FoldersService _foldersService;

        public SitesController(
            DeveloperDbContext dbContext,
            AppsContainer appsContainer,
            SitesService sitesService,
            FoldersService foldersService)
        {
            _dbContext = dbContext;
            _appsContainer = appsContainer;
            _sitesService = sitesService;
            _foldersService = foldersService;
        }

        [Route(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

        [Route(nameof(CreateSite))]
        public async Task<IActionResult> CreateSite(string id)// app id
        {
            var user = await GetCurrentUserAsync();
            var model = new CreateSiteViewModel(user)
            {
                AppId = id
            };
            return View(model);
        }

        [HttpPost]
        [Route(nameof(CreateSite))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSite(CreateSiteViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            var app = await _dbContext.Apps.FindAsync(model.AppId);
            if (app == null)
            {
                return NotFound();
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

        [Route("ViewFiles/{appId}/{siteName}/{**path}")]
        public async Task<IActionResult> ViewFiles(string appId, string siteName, string path) // siteName
        {
            var user = await GetCurrentUserAsync();
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
            {
                return NotFound();
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
                    Path = path
                };
                return View(model);
            }
            catch (AiurUnexceptedResponse e)
            {
                if (e.Code == ErrorType.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    throw e;
                }
            }
        }

        [Route("NewFolder/{appId}/{siteName}/{**path}")]
        public async Task<IActionResult> NewFolder(string appId, string siteName, string path)
        {
            var user = await GetCurrentUserAsync();
            var model = new NewFolderViewModel(user)
            {
                AppId = appId,
                SiteName = siteName,
                Path = path
            };
            return View(model);
        }

        [HttpPost]
        [Route("NewFolder/{appId}/{siteName}/{**path}")]
        public async Task<IActionResult> NewFolder(NewFolderViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
            var app = await _dbContext.Apps.FindAsync(model.AppId);
            if (app == null)
            {
                return NotFound();
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                var data = await _foldersService.CreateNewFolderAsync(token, model.SiteName, model.Path, model.NewFolderName);
                return RedirectToAction(nameof(ViewFiles), new { appId = model.AppId, siteName = model.SiteName, path = model.Path });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}

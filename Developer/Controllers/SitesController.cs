using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models.SitesViewModels;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToProbeServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
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

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

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

        [Route("ViewFiles/{appId}/{siteName}/{**folder}")]
        public async Task<IActionResult> ViewFiles(string appId, string siteName, string folder) // siteName
        {
            var app = await _dbContext.Apps.FindAsync(appId);
            if (app == null)
            {
                return NotFound();
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                var data = await _foldersService.ViewContentAsync(token, siteName, folder);
                return Json(data);
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

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}

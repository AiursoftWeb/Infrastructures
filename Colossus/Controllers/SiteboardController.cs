using Aiursoft.Colossus.Models;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
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
    public class SiteboardController : Controller
    {
        private readonly SitesService _sitesService;
        private readonly AppsContainer _appsContainer;
        private readonly UserManager<ColossusUser> _userManager;

        public SiteboardController(
            SitesService sitesService,
            AppsContainer appsContainer,
            UserManager<ColossusUser> userManager)
        {
            _sitesService = sitesService;
            _appsContainer = appsContainer;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var sites = await _sitesService.ViewMySitesAsync(token);
            if (string.IsNullOrEmpty(user.SiteName) || sites.Sites.Any(t => t.SiteName == user.SiteName))
            {
                return RedirectToAction();
            }
            return Ok();
        }

        public async Task<IActionResult> CreateSite([FromRoute]string id)// app id
        {
            var user = await GetCurrentUserAsync();
            var model = new CreateSiteViewModel(user)
            {

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
            try
            {
                var token = await _appsContainer.AccessToken();
                await _sitesService.CreateNewSiteAsync(token, model.SiteName);
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

        private async Task<ColossusUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}

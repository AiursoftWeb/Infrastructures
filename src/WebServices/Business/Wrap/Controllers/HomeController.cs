using Aiursoft.Archon.SDK.Services;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.Wrap.Models;
using Aiursoft.Wrap.Models.HomeViewModels;
using Aiursoft.Wrapgate.SDK.Models;
using Aiursoft.Wrapgate.SDK.Services.ToWrapgateServer;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Wrap.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly SignInManager<WrapUser> _signInManager;
        private readonly GatewayLocator _gatewayLocator;
        private readonly RecordsService _recordsService;
        private readonly AppsContainer _appsContainer;

        public HomeController(
            SignInManager<WrapUser> signInManager,
            GatewayLocator gatewayLocator,
            RecordsService recordsService,
            AppsContainer appsContainer)
        {
            _signInManager = signInManager;
            _gatewayLocator = gatewayLocator;
            _recordsService = recordsService;
            this._appsContainer = appsContainer;
        }

        [AiurForceAuth(preferController: "Dashboard", preferAction: "Index", justTry: true)]
        public IActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IndexViewModel model)
        {
            var token = await _appsContainer.AccessToken();
            try
            {
                await _recordsService.CreateNewRecordAsync(token, model.NewRecordName, model.Url, new[] { "Anonymous" }, RecordType.Redirect, enabled: true);
            }
            catch (AiurUnexpectedResponse e) when (e.Code == ErrorType.Conflict)
            {
                ModelState.AddModelError(nameof(model.NewRecordName), $"Sorry but the key:'{model.NewRecordName}' already exists. Try another one.");
                return View(nameof(Index), model);
            }
            return View("Created", model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return this.SignOutRootServer(_gatewayLocator.Endpoint, new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
        }
    }
}

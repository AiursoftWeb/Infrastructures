using Aiursoft.Wrap.Models;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Aiursoft.Wrap.Models.HomeViewModels;
using System;
using Aiursoft.Wrapgate.SDK.Services.ToWrapgateServer;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Wrapgate.SDK.Models;

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

        [AiurForceAuth("", "", justTry: true)]
        public IActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IndexViewModel model)
        {
            var token = await _appsContainer.AccessToken();
            await _recordsService.CreateNewRecordAsync(token, model.NewRecordName, model.Url, RecordType.Redirect, enabled: true);
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

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

namespace Aiursoft.Wrap.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly SignInManager<WrapUser> _signInManager;
        private readonly GatewayLocator _gatewayLocator;

        public HomeController(
            SignInManager<WrapUser> signInManager,
            GatewayLocator gatewayLocator)
        {
            _signInManager = signInManager;
            _gatewayLocator = gatewayLocator;
        }

        [AiurForceAuth("", "", justTry: true)]
        public IActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return this.SignOutRootServer(_gatewayLocator.Endpoint, new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
        }
    }
}

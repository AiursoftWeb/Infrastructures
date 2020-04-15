using Aiursoft.Account.Models;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Aiursoft.Account.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly SignInManager<AccountUser> _signInManager;
        private readonly ILogger _logger;
        private readonly GatewayLocator _gatewayLocator;

        public HomeController(
            SignInManager<AccountUser> signInManager,
            ILoggerFactory loggerFactory,
            GatewayLocator gatewayLocator)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _gatewayLocator = gatewayLocator;
        }

        [AiurForceAuth(preferController: "Account", preferAction: "Index", justTry: true)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return this.SignOutRootServer(_gatewayLocator.Endpoint, new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
        }
    }
}

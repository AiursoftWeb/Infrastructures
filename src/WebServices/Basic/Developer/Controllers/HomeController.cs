using Aiursoft.Developer.Models;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly SignInManager<DeveloperUser> _signInManager;
        private readonly ILogger _logger;
        private readonly GatewayLocator _gatewayLocator;

        public HomeController(
            SignInManager<DeveloperUser> signInManager,
            ILoggerFactory loggerFactory,
            GatewayLocator gatewayLocator)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _gatewayLocator = gatewayLocator;
        }

        [AiurForceAuth("", "", justTry: true)]
        public IActionResult Index()
        {
            return View();
        }

        [AiurForceAuth("", "", justTry: true)]
        public IActionResult Error()
        {
            throw new Exception("This is a test error.");
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return this.SignOutRootServer(_gatewayLocator.Endpoint, new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }
    }
}

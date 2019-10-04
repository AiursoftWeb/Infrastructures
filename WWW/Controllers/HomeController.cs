using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.WWW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Aiursoft.WWW.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        public readonly SignInManager<WWWUser> _signInManager;
        public readonly ILogger _logger;
        private readonly ServiceLocation _serviceLocation;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(
            SignInManager<WWWUser> signInManager,
            ILoggerFactory loggerFactory,
            ServiceLocation serviceLocation,
            IStringLocalizer<HomeController> localizer)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _serviceLocation = serviceLocation;
            _localizer = localizer;
        }

        [AiurForceAuth("", "", justTry: true)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return this.SignOutRootServer(_serviceLocation.Gateway, new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }
    }
}

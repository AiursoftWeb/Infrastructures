using Aiursoft.Account.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.SDK.Services;
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
        private readonly ServiceLocation _serviceLocation;

        public HomeController(
            SignInManager<AccountUser> signInManager,
            ILoggerFactory loggerFactory,
            ServiceLocation serviceLocation)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _serviceLocation = serviceLocation;
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
            return this.SignOutRootServer(_serviceLocation.Gateway, new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
        }
    }
}

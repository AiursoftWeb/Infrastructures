using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Aiursoft.Developer.Models;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models;

namespace Aiursoft.Developer.Controllers
{
    public class HomeController : Controller
    {
        public readonly SignInManager<DeveloperUser> _signInManager;
        public readonly ILogger _logger;

        public HomeController(
            SignInManager<DeveloperUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
        }

        [AiurForceAuth("Apps", "Index", justTry: true)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Docs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return this.SignoutRootServer(new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

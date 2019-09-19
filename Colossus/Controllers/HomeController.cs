using Aiursoft.Colossus.Models;
using Aiursoft.Colossus.Models.HomeViewModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ColossusUser> _signInManager;
        private readonly ServiceLocation _serviceLocation;
        private const int _defaultSize = 30 * 1024 * 1024;

        public HomeController(
            IConfiguration configuration,
            SignInManager<ColossusUser> signInManager,
            ServiceLocation serviceLocation)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _serviceLocation = serviceLocation;
        }

        [AiurForceAuth(preferController: "Dashboard", preferAction: "Index", justTry: true)]
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                MaxSize = _defaultSize
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return this.SignOutRootServer(_serviceLocation.API, new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
        }
    }
}

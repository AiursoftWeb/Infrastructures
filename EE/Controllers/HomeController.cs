using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        public readonly SignInManager<EEUser> _signInManager;
        public readonly ILogger _logger;
        public readonly EEDbContext _dbContext;
        private readonly ServiceLocation _serviceLocation;

        public HomeController(
            SignInManager<EEUser> signInManager,
            ILoggerFactory loggerFactory,
            EEDbContext dbContext,
            ServiceLocation serviceLocation)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _dbContext = dbContext;
            _serviceLocation = serviceLocation;
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
            return this.SignOutRootServer(_serviceLocation.API, new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }

        public async Task<IActionResult> Search(string word)
        {
            var results = await _dbContext.Courses.Where(t=>t.Name.Contains(word)).ToListAsync();
            return View();
        }
    }
}

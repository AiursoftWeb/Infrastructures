using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.WWW.Models;
using Aiursoft.Pylon.Attributes;

namespace Aiursoft.WWW.Controllers
{
    public class HomeController : Controller
    {
        public readonly SignInManager<WWWUser> _signInManager;
        public readonly ILogger _logger;
        public readonly EEDbContext _dbContext;
        private readonly ServiceLocation _serviceLocation;

        public HomeController(
            SignInManager<WWWUser> signInManager,
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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return this.SignoutRootServer(_serviceLocation.API, new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.EE.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Aiursoft.Pylon.Models;
using Aiursoft.EE.Data;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.EE.Controllers
{
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

        public async Task<IActionResult> Search(string word)
        {
            var results = await _dbContext.Courses.Where(t=>t.Name.Contains(word)).ToListAsync();
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

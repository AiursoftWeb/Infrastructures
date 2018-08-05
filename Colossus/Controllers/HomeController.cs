using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Colossus.Models;
using Aiursoft.Pylon.Attributes;
using System.IO;
using Aiursoft.Pylon;
using Microsoft.Extensions.Configuration;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon.Models;

namespace Aiursoft.Colossus.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly StorageService _storageService;
        private readonly SignInManager<ColossusUser> _signInManager;
        private readonly ServiceLocation _serviceLocation;

        public HomeController(
            IConfiguration configuration,
            StorageService storageService,
            SignInManager<ColossusUser> signInManager,
            ServiceLocation serviceLocation)
        {
            _configuration = configuration;
            _storageService = storageService;
            _signInManager = signInManager;
            _serviceLocation = serviceLocation;
        }

        [AiurForceAuth("", "", justTry: true)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [FileChecker(MaxSize = 1000 * 1024 * 1024)]
        public async Task<IActionResult> Upload()
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }
            var file = Request.Form.Files.First();
            var path = await _storageService.SaveToOSS(file, Convert.ToInt32(_configuration["ColossusPublicBucketId"]), 3);
            return Json(new
            {
                message = "Uploaded!",
                value = path
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return this.SignoutRootServer(_serviceLocation.API, new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }
    }
}

using Aiursoft.Colossus.Models;
using Aiursoft.Colossus.Models.HomeViewModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly StorageService _storageService;
        private readonly SignInManager<ColossusUser> _signInManager;
        private readonly UserManager<ColossusUser> _userManager;
        private readonly ServiceLocation _serviceLocation;
        private const long _30M = 30 * 1024 * 1024;

        public HomeController(
            IConfiguration configuration,
            StorageService storageService,
            SignInManager<ColossusUser> signInManager,
            UserManager<ColossusUser> userManager,
            ServiceLocation serviceLocation)
        {
            _configuration = configuration;
            _storageService = storageService;
            _signInManager = signInManager;
            _userManager = userManager;
            _serviceLocation = serviceLocation;
        }

        [AiurForceAuth("", "", justTry: true)]
        public IActionResult Index()
        {
            long maxSize = _30M;
            if (User.Identity.IsAuthenticated)
            {
                maxSize = Values.MaxFileSize;
            }
            var model = new IndexViewModel
            {
                MaxSize = maxSize
            };
            return View(model);
        }

        [HttpPost]
        [FileChecker(MaxSize = 1000 * 1024 * 1024)]
        public async Task<IActionResult> Upload()
        {
            // Anonymous user but try to upload a large file.
            if (!User.Identity.IsAuthenticated && HttpContext.Request.Form.Files.First().Length > _30M)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }
            var user = await GetCurrentUserAsync();
            var day = 3;
            if (user != null)
            {
                day = 30;
            }
            var file = Request.Form.Files.First();
            var path = await _storageService.SaveToOSS(file, Convert.ToInt32(_configuration["ColossusPublicBucketId"]), day);
            return Json(new
            {
                message = "Uploaded!",
                value = path
            });
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return this.SignoutRootServer(_serviceLocation.API, new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }

        private async Task<ColossusUser> GetCurrentUserAsync()
        {
            if (User.Identity.Name == null)
            {
                return null;
            }
            return await _userManager.FindByNameAsync(User.Identity.Name);
        }
    }
}

using Aiursoft.Colossus.Models;
using Aiursoft.Colossus.Models.HomeViewModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
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
        public const int DefaultSize  = 30 * 1024 * 1024;

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

        [AiurForceAuth(preferController: "Dashboard", preferAction: "Index", justTry: true)]
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                MaxSize = DefaultSize
            };
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [FileChecker(MaxSize = DefaultSize)]
        [APIModelStateChecker]
        public async Task<IActionResult> Upload()
        {
            if (HttpContext.Request.Form.Files.First().Length > DefaultSize)
            {
                return Unauthorized();
            }
            var file = Request.Form.Files.First();
            var model = await _storageService
                .SaveToOSS(file, Convert.ToInt32(_configuration["ColossusPublicBucketId"]), 3);
            return Json(new AiurValue<string>(model.Path)
            {
                Code = ErrorType.Success,
                Message = "Uploaded!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return this.SignOutRootServer(_serviceLocation.API, new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
        }

        private async Task<ColossusUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}

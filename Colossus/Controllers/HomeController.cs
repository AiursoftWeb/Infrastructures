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
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly StorageService _storageService;
        private readonly SignInManager<ColossusUser> _signInManager;
        private readonly ServiceLocation _serviceLocation;
        private const int _defaultSize = 30 * 1024 * 1024;

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

        [AiurForceAuth(preferController: "Siteboard", preferAction: "Index", justTry: true)]
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                MaxSize = _defaultSize
            };
            return View(model);
        }

        [HttpPost]
        [APIExpHandler]
        [FileChecker(MaxSize = _defaultSize)]
        [APIModelStateChecker]
        public async Task<IActionResult> Upload()
        {
            if (HttpContext.Request.Form.Files.First().Length > _defaultSize)
            {
                return Unauthorized();
            }
            var file = Request.Form.Files.First();
            var model = await _storageService
                .SaveToProbe(file, _configuration["ColossusPublicSiteName"], $"{DateTime.UtcNow.Date.ToString("yyyy-MM-dd")}", SaveFileOptions.SourceName);
            return Json(new AiurValue<string>(model.InternetPath)
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
    }
}

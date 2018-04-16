using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.API.Data;
using Microsoft.Extensions.Logging;
using Aiursoft.API.Services;
using Aiursoft.API.Models;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon.Models;
using Aiursoft.API.Models.HomeViewModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Microsoft.Extensions.Localization;

namespace Aiursoft.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<APIUser> _userManager;
        private readonly SignInManager<APIUser> _signInManager;
        private readonly ILogger _logger;
        private readonly APIDbContext _dbContext;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(
            UserManager<APIUser> userManager,
            SignInManager<APIUser> signInManager,
            ILoggerFactory loggerFactory,
            APIDbContext _context,
            IStringLocalizer<HomeController> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _dbContext = _context;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var cuser = await GetCurrentUserAsync();
            return Json(new IndexViewModel
            {
                Signedin = User.Identity.IsAuthenticated,
                ServerTime = DateTime.Now,
                code  = ErrorType.Success,
                message = "Server started successfully!",
                Local = _localizer["en"],
                User = cuser
            });
        }

        private async Task<APIUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
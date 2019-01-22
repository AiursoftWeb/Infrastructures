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
using Aiursoft.Pylon.Services;

namespace Aiursoft.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<APIUser> _userManager;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly RSAService _rsaService;
        private readonly APIDbContext _dbContext;

        public HomeController(
            UserManager<APIUser> userManager,
            IStringLocalizer<HomeController> localizer,
            RSAService rsaService,
            APIDbContext dbContext)
        {
            _userManager = userManager;
            _localizer = localizer;
            _rsaService = rsaService;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var cuser = await GetCurrentUserAsync();
            await _dbContext.Entry(cuser).Collection(t => t.Emails).LoadAsync();
            return Json(new IndexViewModel
            {
                Signedin = User.Identity.IsAuthenticated,
                ServerTime = DateTime.UtcNow,
                Code = ErrorType.Success,
                Message = "Server started successfully!",
                Local = _localizer["en"],
                User = cuser,
                PublicKey = _rsaService._publicKey
            });
        }

        private async Task<APIUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
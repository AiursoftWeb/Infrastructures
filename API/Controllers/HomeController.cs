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
using Microsoft.EntityFrameworkCore;

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

        private Task<APIUser> GetCurrentUserAsync()
        {
            return _dbContext
                .Users
                .Include(t => t.Emails)
                .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}
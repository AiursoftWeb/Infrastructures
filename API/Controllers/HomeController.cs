using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.API.Data;
using Aiursoft.API.Models;
using Aiursoft.Pylon.Models;
using Aiursoft.API.Models.HomeViewModels;
using Microsoft.Extensions.Localization;
using Aiursoft.Pylon.Services;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly APIDbContext _dbContext;

        public HomeController(
            IStringLocalizer<HomeController> localizer,
            APIDbContext dbContext)
        {
            _localizer = localizer;
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
                User = cuser
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
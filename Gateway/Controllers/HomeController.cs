using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Gateway.Models.HomeViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Edi.Captcha;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Controllers
{
    [LimitPerMin]
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly GatewayDbContext _dbContext;
        private readonly ISessionBasedCaptcha _captcha;

        public HomeController(
            IStringLocalizer<HomeController> localizer,
            GatewayDbContext dbContext,
            ISessionBasedCaptcha captcha)
        {
            _localizer = localizer;
            _dbContext = dbContext;
            _captcha = captcha;
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

        private Task<GatewayUser> GetCurrentUserAsync()
        {
            return _dbContext
                .Users
                .Include(t => t.Emails)
                .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }

        [AiurNoCache]
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            return _captcha.GenerateCaptchaImageFileStream(HttpContext.Session, 100, 33);
        }
    }
}
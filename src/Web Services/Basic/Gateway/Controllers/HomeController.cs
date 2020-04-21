using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Gateway.Models.HomeViewModels;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.SDK.Attributes;
using Edi.Captcha;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IActionResult> Report()
        {
            var oneYearBefore = DateTime.UtcNow - TimeSpan.FromDays(90);
            var severnDaysBefore = DateTime.UtcNow - TimeSpan.FromDays(7);
            var oneDayBefore = DateTime.UtcNow - TimeSpan.FromDays(1);
            return Json(new
            {
                ActiveUsers7Days = await _dbContext.AuditLogs
                    .Where(t => t.HappenTime > severnDaysBefore)
                    .Select(t => t.UserId)
                    .Distinct()
                    .CountAsync(),
                ActiveUsers1Day = await _dbContext.AuditLogs
                    .Where(t => t.HappenTime > oneDayBefore)
                    .Select(t => t.UserId)
                    .Distinct()
                    .CountAsync(),
                NewUsers7Days = await _dbContext.Users
                    .Where(t => t.AccountCreateTime > severnDaysBefore)
                    .CountAsync(),
                NewUsers1Day = await _dbContext.Users
                    .Where(t => t.AccountCreateTime > oneDayBefore)
                    .CountAsync(),
                LongTermUsers = _dbContext.AuditLogs
                    .GroupBy(t => t.UserId)
                    .Select(t => t.Max(a => a.HappenTime) - t.Min(a => a.HappenTime))
                    .AsEnumerable()
                    .Count(t => t.TotalDays > 15),
                SpamUsers = await _dbContext.Users
                    .Where(t => !t.Emails.Any(p => p.Validated))
                    .Where(t => !t.AuditLogs.Any(p => p.HappenTime > oneYearBefore))
                    .CountAsync(),
                TotalUsers = await _dbContext.Users.CountAsync(),
                TotalEmailAddress = await _dbContext.UserEmails.CountAsync(),
                TotalVerifiedEmailAddress = await _dbContext
                    .UserEmails
                    .Where(t => t.Validated)
                    .CountAsync(),
                AverageUserGrantedApps = await _dbContext
                    .LocalAppGrant.CountAsync() * 1.0 /
                    await _dbContext.Users.CountAsync(),
                SocialAccounts = await _dbContext.ThirdPartyAccounts
                    .GroupBy(t => t.ProviderName)
                    .Select(t => KeyValuePair.Create(t.Key, t.Count()))
                    .ToListAsync()
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
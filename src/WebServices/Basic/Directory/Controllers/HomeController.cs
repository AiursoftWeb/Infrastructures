﻿using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Directory.Models;
using Aiursoft.Directory.SDK.Models.API.HomeViewModels;
using Aiursoft.SDK.Attributes;
using Aiursoft.CSTools.Tools;
using Edi.Captcha;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.Services;

namespace Aiursoft.Directory.Controllers;

public class HomeController : ControllerBase
{
    private readonly ISessionBasedCaptcha _captcha;
    private readonly DirectoryDbContext _dbContext;
    private readonly IStringLocalizer<HomeController> _localizer;
    private readonly PrivateKeyStore _privateKeyStore;

    public HomeController(
        PrivateKeyStore privateKeyStore,
        IStringLocalizer<HomeController> localizer,
        DirectoryDbContext dbContext,
        ISessionBasedCaptcha captcha)
    {
        _privateKeyStore = privateKeyStore;
        _localizer = localizer;
        _dbContext = dbContext;
        _captcha = captcha;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await GetCurrentUserAsync();
        return this.Protocol(new DirectoryServerConfiguration
        {
            SignedIn = User.Identity?.IsAuthenticated ?? false,
            ServerTime = DateTime.UtcNow,
            Code = Code.ResultShown,
            Message = "Server started successfully!",
            Local = _localizer["en"],
            Exponent = _privateKeyStore.GetPublicKey().Exponent!.BytesToBase64(),
            Modulus = _privateKeyStore.GetPublicKey().Modulus!.BytesToBase64(),
            User = currentUser
        });
    }

    //public async Task<IActionResult> Report()
    //{
    //    var oneYearBefore = DateTime.UtcNow - TimeSpan.FromDays(90);
    //    var severnDaysBefore = DateTime.UtcNow - TimeSpan.FromDays(7);
    //    var oneDayBefore = DateTime.UtcNow - TimeSpan.FromDays(1);
    //    return this.Protocol(new
    //    {
    //        ActiveUsers7Days = await _dbContext.AuditLogs
    //            .Where(t => t.HappenTime > severnDaysBefore)
    //            .Select(t => t.UserId)
    //            .Distinct()
    //            .CountAsync(),
    //        ActiveUsers1Day = await _dbContext.AuditLogs
    //            .Where(t => t.HappenTime > oneDayBefore)
    //            .Select(t => t.UserId)
    //            .Distinct()
    //            .CountAsync(),
    //        NewUsers7Days = await _dbContext.Users
    //            .Where(t => t.AccountCreateTime > severnDaysBefore)
    //            .CountAsync(),
    //        NewUsers1Day = await _dbContext.Users
    //            .Where(t => t.AccountCreateTime > oneDayBefore)
    //            .CountAsync(),
    //        LongTermUsers = _dbContext.AuditLogs
    //            .GroupBy(t => t.UserId)
    //            .Select(t => t.Max(a => a.HappenTime) - t.Min(a => a.HappenTime))
    //            .AsEnumerable()
    //            .Count(t => t.TotalDays > 15),
    //        SpamUsers = await _dbContext.Users
    //            .Where(t => !t.Emails.Any(p => p.Validated))
    //            .Where(t => !t.AuditLogs.Any(p => p.HappenTime > oneYearBefore))
    //            .CountAsync(),
    //        TotalUsers = await _dbContext.Users.CountAsync(),
    //        TotalEmailAddress = await _dbContext.UserEmails.CountAsync(),
    //        TotalVerifiedEmailAddress = await _dbContext
    //            .UserEmails
    //            .Where(t => t.Validated)
    //            .CountAsync(),
    //        AverageUserGrantedApps = await _dbContext
    //            .LocalAppGrant.CountAsync() * 1.0 /
    //            await _dbContext.Users.CountAsync(),
    //        SocialAccounts = await _dbContext.ThirdPartyAccounts
    //            .GroupBy(t => t.ProviderName)
    //            .Select(t => KeyValuePair.Create(t.Key, t.Count()))
    //            .ToListAsync()
    //    });
    //}

    private Task<DirectoryUser> GetCurrentUserAsync()
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
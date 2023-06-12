using System;
using System.Threading.Tasks;
using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aiursoft.Developer.Controllers;

[LimitPerMin]
public class HomeController : Controller
{
    private readonly DirectoryConfiguration _directoryLocator;
    private readonly ILogger _logger;
    private readonly DeveloperDbContext _context;
    private readonly SignInManager<DeveloperUser> _signInManager;

    public HomeController(
        DeveloperDbContext context,
        SignInManager<DeveloperUser> signInManager,
        ILoggerFactory loggerFactory,
        IOptions<DirectoryConfiguration> directoryLocator)
    {
        _context = context;
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<HomeController>();
        _directoryLocator = directoryLocator.Value;
    }

    [AiurForceAuth("", "", true)]
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> OneTimeExport()
    {
        var apps = await _context.Apps.ToListAsync();
        return Json(apps);
    }

    [AiurForceAuth("", "", true)]
    public IActionResult Error()
    {
        throw new Exception("This is a test view error for debugging.");
    }

    [HttpPost]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation(4, "User logged out");
        return this.SignOutRootServer(_directoryLocator.Instance,
            new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
    }
}
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.WWW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aiursoft.WWW.Controllers;


public class HomeController : Controller
{
    private readonly ILogger _logger;
    private readonly DirectoryConfiguration _directoryLocator;
    private readonly SignInManager<WWWUser> _signInManager;

    public HomeController(
        SignInManager<WWWUser> signInManager,
        ILoggerFactory loggerFactory,
        IOptions<DirectoryConfiguration> serviceLocation)
    {
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<HomeController>();
        _directoryLocator = serviceLocation.Value;
    }

    [AiurForceAuth("", "", true)]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation(4, "User logged out");
        return this.SignOutRootServer(_directoryLocator.Instance, $"Home/{nameof(Index)}");
    }
}
using System;
using System.Threading.Tasks;
using Aiursoft.Portal.Models;
using Aiursoft.Directory.SDK.Configuration;

using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aiursoft.Portal.Controllers;


public class HomeController : Controller
{
    private readonly DirectoryConfiguration _directoryLocator;
    private readonly ILogger _logger;
    private readonly SignInManager<PortalUser> _signInManager;

    public HomeController(
        SignInManager<PortalUser> signInManager,
        ILoggerFactory loggerFactory,
        IOptions<DirectoryConfiguration> directoryLocator)
    {
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<HomeController>();
        _directoryLocator = directoryLocator.Value;
    }

    [AiurForceAuth("", "", true)]
    public IActionResult Index()
    {
        return View();
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
        return this.SignOutRootServer(_directoryLocator.Instance, $"Home/{nameof(Index)}");
    }
}
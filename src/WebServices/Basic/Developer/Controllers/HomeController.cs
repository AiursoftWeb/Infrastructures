using System;
using System.Threading.Tasks;
using Aiursoft.Developer.Models;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Developer.Controllers;

[LimitPerMin]
public class HomeController : Controller
{
    private readonly GatewayLocator _gatewayLocator;
    private readonly ILogger _logger;
    private readonly SignInManager<DeveloperUser> _signInManager;

    public HomeController(
        SignInManager<DeveloperUser> signInManager,
        ILoggerFactory loggerFactory,
        GatewayLocator gatewayLocator)
    {
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<HomeController>();
        _gatewayLocator = gatewayLocator;
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
        _logger.LogInformation(4, "User logged out.");
        return this.SignOutRootServer(_gatewayLocator.Endpoint,
            new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
    }
}
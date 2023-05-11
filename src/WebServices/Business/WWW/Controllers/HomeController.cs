using System.Threading.Tasks;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.WWW.Models;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Aiursoft.WWW.Controllers;

[LimitPerMin]
public class HomeController : Controller
{
    private readonly ILogger _logger;
    private readonly GatewayLocator _serviceLocation;
    private readonly SignInManager<WWWUser> _signInManager;

    public HomeController(
        SignInManager<WWWUser> signInManager,
        ILoggerFactory loggerFactory,
        GatewayLocator serviceLocation)
    {
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<HomeController>();
        _serviceLocation = serviceLocation;
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
        _logger.LogInformation(4, "User logged out.");
        return this.SignOutRootServer(_serviceLocation.Endpoint,
            new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
    }
}
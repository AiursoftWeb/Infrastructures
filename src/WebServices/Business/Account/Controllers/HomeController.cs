using System.Threading.Tasks;
using Aiursoft.Account.Models;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Account.Controllers;

[LimitPerMin]
public class HomeController : Controller
{
    private readonly DirectoryContext _gatewayLocator;
    private readonly ILogger _logger;
    private readonly SignInManager<AccountUser> _signInManager;

    public HomeController(
        SignInManager<AccountUser> signInManager,
        ILoggerFactory loggerFactory,
        DirectoryContext gatewayLocator)
    {
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<HomeController>();
        _gatewayLocator = gatewayLocator;
    }

    [AiurForceAuth("Account", "Index", true)]
    public IActionResult Index()
    {
        return View();
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
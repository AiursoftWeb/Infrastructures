using System.Threading.Tasks;
using Aiursoft.Account.Models;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Handler.Attributes;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aiursoft.Account.Controllers;

[LimitPerMin]
public class HomeController : Controller
{
    private readonly DirectoryConfiguration _directoryConfig;
    private readonly ILogger _logger;
    private readonly SignInManager<AccountUser> _signInManager;

    public HomeController(
        SignInManager<AccountUser> signInManager,
        ILoggerFactory loggerFactory,
        IOptions<DirectoryConfiguration> directoryConfig)
    {
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<HomeController>();
        _directoryConfig = directoryConfig.Value;
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
        return this.SignOutRootServer(_directoryConfig.Instance,
            new AiurUrl(string.Empty, "Home", nameof(Index), new { }));
    }
}
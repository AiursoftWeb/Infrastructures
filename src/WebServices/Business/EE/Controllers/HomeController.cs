using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.Identity;
using Aiursoft.Identity.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aiursoft.EE.Controllers;


public class HomeController : Controller
{
    public readonly EEDbContext _dbContext;
    public readonly ILogger _logger;
    private readonly DirectoryConfiguration _serviceLocation;
    public readonly SignInManager<EEUser> _signInManager;

    public HomeController(
        SignInManager<EEUser> signInManager,
        ILoggerFactory loggerFactory,
        EEDbContext dbContext,
        IOptions<DirectoryConfiguration> serviceLocation)
    {
        _signInManager = signInManager;
        _logger = loggerFactory.CreateLogger<HomeController>();
        _dbContext = dbContext;
        _serviceLocation = serviceLocation.Value;
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
        return this.SignOutRootServer(_serviceLocation.Instance, $"Home/{nameof(Index)}");
    }

    public async Task<IActionResult> Search(string word)
    {
        await _dbContext.Courses.Where(t => t.Name.Contains(word)).ToListAsync();
        return View();
    }
}
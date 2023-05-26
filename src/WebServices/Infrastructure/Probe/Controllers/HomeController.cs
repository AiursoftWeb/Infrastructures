using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.Probe.Controllers;

[LimitPerMin]
public class HomeController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return this.Protocol(new ProbeDownloadPatternConfig
        {
            OpenPattern = _configuration["OpenPattern"],
            DownloadPattern = _configuration["DownloadPattern"],
            PlayerPattern = _configuration["PlayerPattern"],
            Code = ErrorType.Success,
            Message = "Welcome to Probe!"
        });
    }
}
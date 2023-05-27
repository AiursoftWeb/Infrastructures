using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aiursoft.Probe.Controllers;

[LimitPerMin]
public class HomeController : ControllerBase
{
    private readonly ProbeDownloadPatternConfig _configuration;

    public HomeController(IOptions<ProbeDownloadPatternConfig> config)
    {
        _configuration = config.Value;
    }

    public IActionResult Index()
    {
        return this.Protocol(new ProbeDownloadPatternConfig
        {
            OpenPattern = _configuration.OpenPattern,
            DownloadPattern = _configuration.DownloadPattern,
            PlayerPattern = _configuration.PlayerPattern,
            Code = ErrorType.Success,
            Message = "Welcome to Probe!"
        });
    }
}
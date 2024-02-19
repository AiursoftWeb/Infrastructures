using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Probe.SDK.Models.HomeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aiursoft.Probe.Controllers;

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
            Code = Code.ResultShown,
            Message = "Welcome to Probe!"
        });
    }
}
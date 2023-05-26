using Aiursoft.Handler.Models;

namespace Aiursoft.Probe.SDK.Models.HomeViewModels;

public class ProbeDownloadPatternConfig : AiurProtocol
{
    public string DownloadPattern { get; set; }
    public string PlayerPattern { get; set; }
    public string OpenPattern { get; set; }
}
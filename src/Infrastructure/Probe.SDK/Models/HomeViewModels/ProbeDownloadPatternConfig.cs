using Aiursoft.AiurProtocol.Models;

namespace Aiursoft.Probe.SDK.Models.HomeViewModels;

public class ProbeDownloadPatternConfig : AiurResponse
{
    public string DownloadPattern { get; set; }
    public string PlayerPattern { get; set; }
    public string OpenPattern { get; set; }
}
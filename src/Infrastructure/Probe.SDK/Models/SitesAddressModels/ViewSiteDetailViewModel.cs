using Aiursoft.AiurProtocol;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels;

public class ViewSiteDetailViewModel : AiurResponse
{
    public string AppId { get; set; }
    public Site Site { get; set; }
    public long Size { get; set; }
}
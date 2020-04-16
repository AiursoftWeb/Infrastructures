using Aiursoft.Handler.Models;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels
{
    public class ViewSiteDetailViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public Site Site { get; set; }
        public long Size { get; set; }
    }
}

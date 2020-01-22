using Aiursoft.Handler.Models;

namespace Aiursoft.SDK.Models.Probe.SitesAddressModels
{
    public class ViewSiteDetailViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public Site Site { get; set; }
        public long Size { get; set; }
    }
}

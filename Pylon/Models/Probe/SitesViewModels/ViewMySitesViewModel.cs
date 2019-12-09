using System.Collections.Generic;

namespace Aiursoft.Pylon.Models.Probe.SitesViewModels
{
    public class ViewMySitesViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public List<Site> Sites { get; set; }
    }
}

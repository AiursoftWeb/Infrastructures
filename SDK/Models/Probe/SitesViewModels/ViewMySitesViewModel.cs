using Aiursoft.XelNaga.Models;
using System.Collections.Generic;

namespace Aiursoft.SDK.Models.Probe.SitesViewModels
{
    public class ViewMySitesViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public List<Site> Sites { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.Probe.SitesViewModels
{
    public class ViewMySitesViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public List<Site> Sites { get; set; }
    }
}

using System.Collections.Generic;
using Aiursoft.Handler.Models;

namespace Aiursoft.Probe.SDK.Models.SitesViewModels;

public class ViewMySitesViewModel : AiurProtocol
{
    public string AppId { get; set; }
    public List<Site> Sites { get; set; }
}
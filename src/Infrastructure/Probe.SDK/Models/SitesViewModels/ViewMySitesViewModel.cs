using System.Collections.Generic;
using Aiursoft.AiurProtocol;

namespace Aiursoft.Probe.SDK.Models.SitesViewModels;

public class ViewMySitesViewModel : AiurResponse
{
    public string AppId { get; set; }
    public List<Site> Sites { get; set; }
}
using Aiursoft.AiurProtocol.Models;

namespace Aiursoft.Probe.SDK.Models.SitesViewModels;

public class ViewMySitesViewModel : AiurResponse
{
    public string AppId { get; set; }
    public List<Site> Sites { get; set; }
}
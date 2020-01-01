using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.Probe.SitesAddressModels
{
    public class ViewMySitesAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}

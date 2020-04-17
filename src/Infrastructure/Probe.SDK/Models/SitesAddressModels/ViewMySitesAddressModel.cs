using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels
{
    public class ViewMySitesAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}

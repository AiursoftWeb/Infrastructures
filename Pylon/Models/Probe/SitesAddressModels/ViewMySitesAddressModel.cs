using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.SitesAddressModels
{
    public class ViewMySitesAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}

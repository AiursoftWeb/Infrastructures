using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.SitesAddressModels
{
    public class DeleteSiteAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
    }
}

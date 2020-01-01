using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.Probe.SitesAddressModels
{
    public class DeleteSiteAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
    }
}

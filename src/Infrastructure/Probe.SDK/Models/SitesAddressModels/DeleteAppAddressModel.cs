using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels
{
    public class DeleteAppAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string AppId { get; set; }
    }
}

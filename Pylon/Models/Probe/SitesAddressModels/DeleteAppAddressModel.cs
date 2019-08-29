using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.SitesAddressModels
{
    public class DeleteAppAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string AppId { get; set; }
    }
}

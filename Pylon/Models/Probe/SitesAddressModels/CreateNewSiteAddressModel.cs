using Aiursoft.Pylon.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.SitesAddressModels
{
    public class CreateNewSiteAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidFolderName]
        public string NewSiteName { get; set; }
    }
}

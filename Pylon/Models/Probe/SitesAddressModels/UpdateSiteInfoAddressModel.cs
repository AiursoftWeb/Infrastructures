using Aiursoft.Pylon.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.SitesAddressModels
{
    public class UpdateSiteInfoAddressModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        [ValidDomainName]
        public string OldSiteName { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [ValidDomainName]
        public string NewSiteName { get; set; }

        [Required]
        public bool OpenToUpload { get; set; }

        [Required]
        public bool OpenToDownload { get; set; }
    }
}

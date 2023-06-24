using System.ComponentModel.DataAnnotations;
using Aiursoft.XelNaga.Attributes;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels;

public class UpdateSiteInfoAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required] [ValidDomainName] public string OldSiteName { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(5)]
    [ValidDomainName]
    public string NewSiteName { get; set; }

    [Required] public bool OpenToUpload { get; set; }

    [Required] public bool OpenToDownload { get; set; }
}
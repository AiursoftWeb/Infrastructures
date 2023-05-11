using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels;

public class CreateNewSiteAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(5)]
    [ValidDomainName]
    public string NewSiteName { get; set; }

    [Required] public bool OpenToUpload { get; set; }

    [Required] public bool OpenToDownload { get; set; }
}
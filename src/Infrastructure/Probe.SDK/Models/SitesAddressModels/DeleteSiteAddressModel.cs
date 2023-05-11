using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels;

public class DeleteSiteAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required] public string SiteName { get; set; }
}
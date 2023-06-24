using System.ComponentModel.DataAnnotations;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels;

public class DeleteAppAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required] [IsGuidOrEmpty] public string AppId { get; set; }
}
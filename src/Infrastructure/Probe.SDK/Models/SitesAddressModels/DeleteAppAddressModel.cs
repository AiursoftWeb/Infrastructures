using Aiursoft.SDKTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.SitesAddressModels
{
    public class DeleteAppAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        [IsGuidOrEmpty]
        public string AppId { get; set; }
    }
}

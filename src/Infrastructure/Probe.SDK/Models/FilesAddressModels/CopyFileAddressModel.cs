using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.FilesAddressModels
{
    public class CopyFileAddressModel
    {
        [Required]
        [FromQuery]
        public string AccessToken { get; set; }
        [Required]
        [FromQuery]
        public string SiteName { get; set; }
        [Required]
        [FromRoute]
        public string FolderNames { get; set; }
        [Required]
        [FromQuery]
        public string TargetSiteName { get; set; }
        [Required]
        [FromQuery]
        public string TargetFolderNames { get; set; }
    }
}

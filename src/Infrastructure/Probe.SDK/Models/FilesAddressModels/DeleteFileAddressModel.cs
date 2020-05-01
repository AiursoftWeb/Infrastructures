using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.FilesAddressModels
{
    public class DeleteFileAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
        [Required]
        [FromRoute]
        public string FolderNames { get; set; }
    }
}

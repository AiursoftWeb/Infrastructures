using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.FilesAddressModels
{
    public class UploadFileAddressModel
    {
#warning Rename this to token in the next Kahla release!
        public string PBToken { get; set; }
        [Required]
        [FromRoute]
        public string SiteName { get; set; }
        [FromRoute]
        public string FolderNames { get; set; }
        public bool RecursiveCreate { get; set; }
    }
}

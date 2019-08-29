using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.FilesAddressModels
{
    public class DeleteFileAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
        [Required]
        public string FolderNames { get; set; }
    }
}

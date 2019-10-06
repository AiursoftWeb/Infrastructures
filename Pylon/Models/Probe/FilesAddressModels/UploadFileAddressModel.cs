using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.FilesAddressModels
{
    public class UploadFileAddressModel
    {
        public string PBToken { get; set; }
        [Required]
        public string SiteName { get; set; }
        public string FolderNames { get; set; }
        public bool RecursiveCreate { get; set; }
    }
}

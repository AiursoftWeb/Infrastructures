using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.FoldersAddressModels
{
    public class MoveFolderAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
        [Required]
        public string FolderNames { get; set; }
        [Required]
        public string NewFolderNames { get; set; }
    }
}

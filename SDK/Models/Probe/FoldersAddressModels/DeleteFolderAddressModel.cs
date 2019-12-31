using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.Probe.FoldersAddressModels
{
    public class DeleteFolderAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
        public string FolderNames { get; set; }
    }
}

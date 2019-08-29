using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Probe.FoldersAddressModels
{
    public class ViewContentAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
        public string FolderNames { get; set; }
    }
}

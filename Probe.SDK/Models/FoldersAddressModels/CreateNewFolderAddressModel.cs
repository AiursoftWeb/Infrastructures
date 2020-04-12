using Aiursoft.SDKTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Probe.SDK.Models.FoldersAddressModels
{
    public class CreateNewFolderAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string SiteName { get; set; }
        public string FolderNames { get; set; }
        [Required]
        [ValidFolderName]
        public string NewFolderName { get; set; }
        public bool RecursiveCreate { get; set; } = false;
    }
}

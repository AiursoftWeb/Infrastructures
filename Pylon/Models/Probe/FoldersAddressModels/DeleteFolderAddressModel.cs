using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Probe.FoldersAddressModels
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

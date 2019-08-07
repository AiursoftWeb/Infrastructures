using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Probe.FoldersAddressModels
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

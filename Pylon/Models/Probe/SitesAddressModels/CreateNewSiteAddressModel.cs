using Aiursoft.Pylon.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Probe.SitesAddressModels
{
    public class CreateNewSiteAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        [MaxLength(50)]
        [ValidFolderName]
        public string NewSiteName { get; set; }
    }
}

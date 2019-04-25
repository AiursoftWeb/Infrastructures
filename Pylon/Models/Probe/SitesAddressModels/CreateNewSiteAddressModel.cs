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
        public string NewSiteName { get; set; }
    }
}

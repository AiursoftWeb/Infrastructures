using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Probe.SitesAddressModels
{
    public class ViewMySitesAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}

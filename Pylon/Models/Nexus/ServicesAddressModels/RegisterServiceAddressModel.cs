using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Nexus.ServicesAddressModels
{
    public class RegisterServiceAddressModel : GetServiceAddressModel
    {
        [Url]
        [Required]
        [Range(2, 120)]
        public string ServiceAddress { get; set; }
    }
}

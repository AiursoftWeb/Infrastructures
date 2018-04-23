using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.Nexus.ServicesAddressModels
{
    public class GetServiceAddressModel
    {
        [Required]
        [Range(2, 20)]
        public string ServiceName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.ForApps.AddressModels
{
    public class AuthResultAddressModel
    {
        public string State { get; set; }
        [Required]
        public int Code { get; set; }
    }
}

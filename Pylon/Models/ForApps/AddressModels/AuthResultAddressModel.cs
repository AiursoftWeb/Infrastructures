using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.ForApps.AddressModels
{
    public class AuthResultAddressModel
    {
        public string state { get; set; }
        [Required]
        public int code { get; set; }
    }
}

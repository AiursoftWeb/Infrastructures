using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.AccountAddressModels
{
    public class CodeToOpenIdAddressModel
    {
        [Required]
        public virtual string AccessToken { get; set; }
        [Required]
        public virtual int Code { get; set; }
    }
}

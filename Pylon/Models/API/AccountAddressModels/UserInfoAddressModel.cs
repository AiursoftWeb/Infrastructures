using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.AccountAddressModels
{
    public class UserInfoAddressModel
    {
        [Required]
        public virtual string AccessToken { get; set; }
        [Required]
        public virtual string OpenId { get; set; }
    }
}

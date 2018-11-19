using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.OAuthAddressModels
{
    public class UserInfoAddressModel
    {
        [Required]
        public virtual string access_token { get; set; }
        [Required]
        public virtual string openid { get; set; }
        public virtual string lang { get; set; }
    }
}

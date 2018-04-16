using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.OAuthAddressModels
{
    public class UserInfoAddressModel
    {
        public virtual string access_token { get; set; }
        public virtual string openid { get; set; }
        public virtual string lang { get; set; }
    }
}

using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models
{
    public class AppGrant : Grant
    {
        [JsonIgnore]
        public int AppGrantId { get; set; }
        [JsonIgnore]
        public AccountUser User { get; set; }

        public override AiurUserBase UserInfo => User;
    }
}

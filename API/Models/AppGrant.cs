using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Newtonsoft.Json;

namespace Aiursoft.API.Models
{
    public class AppGrant : Grant
    {
        [JsonIgnore]
        public int AppGrantId { get; set; }
        [JsonIgnore]
        public APIUser User { get; set; }

        public override AiurUserBase UserInfo => User;
    }
}

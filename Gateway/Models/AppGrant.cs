using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.Models
{
    public class AppGrant : Grant
    {
        [JsonIgnore]
        public int AppGrantId { get; set; }
        [JsonIgnore]
        public GatewayUser User { get; set; }

        public override AiurUserBase UserInfo => User;
    }
}

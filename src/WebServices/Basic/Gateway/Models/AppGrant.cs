using Aiursoft.Gateway.SDK.Models;
using Aiursoft.Gateway.SDK.Models.API;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.Models;

public class AppGrant : Grant
{
    [JsonIgnore] public int AppGrantId { get; set; }

    [JsonIgnore] public GatewayUser User { get; set; }

    public override AiurUserBase UserInfo => User;
}
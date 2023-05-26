using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.API;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.Models;

public class AppGrant : Grant
{
    [JsonIgnore] public int AppGrantId { get; set; }

    [JsonIgnore] public DirectoryUser User { get; set; }

    public override AiurUserBase UserInfo => User;
}
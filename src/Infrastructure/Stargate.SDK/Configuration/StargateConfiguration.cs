using System.Text.RegularExpressions;

namespace Aiursoft.Stargate.SDK.Configuration;

public class StargateConfiguration
{
    public string Instance { get; set; }

    public string GetListenEndpoint()
    {
        var wsEndpoint = Regex.Replace(Instance, @"^http://(.*)$", @"ws://$1");
        var wssEndpoint = Regex.Replace(wsEndpoint, @"^https://(.*)$", @"wss://$1");
        return wssEndpoint;
    }
}

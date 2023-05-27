using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Aiursoft.Warpgate.SDK.Configuration;
using Aiursoft.Warpgate.SDK.Models.ViewModels;

namespace Aiursoft.Warpgate.SDK.Services;

public class WarpgateSettingsFetcher : ISingletonDependency
{
    private WarpgatePatternConfig _warpgateServerConfig;
    private readonly WarpgateConfiguration _warpgateConfiguration;

    public WarpgateSettingsFetcher(IOptions<WarpgateConfiguration> probeConfiguration)
    {
        _warpgateConfiguration = probeConfiguration.Value;
    }

    public async Task<WarpgatePatternConfig> GetServerConfig()
    {
        if (_warpgateServerConfig == null)
        {
            var serverConfigString = await SimpleHttp.DownloadAsString(_warpgateConfiguration.Instance);
            _warpgateServerConfig = JsonConvert.DeserializeObject<WarpgatePatternConfig>(serverConfigString);
        }

        return _warpgateServerConfig;
    }
}

using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Aiursoft.Warpgate.SDK.Configuration;
using Aiursoft.Warpgate.SDK.Models.ViewModels;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.Warpgate.SDK.Services;

public class WarpgateSettingsFetcher : ISingletonDependency
{
    private readonly ApiProxyService _apiProxyService;
    
    // TODO: Avoid private variable, use cache service.
    private WarpgatePatternConfig _warpgateServerConfig;
    private readonly WarpgateConfiguration _warpgateConfiguration;

    public WarpgateSettingsFetcher(
        ApiProxyService apiProxyService,
        IOptions<WarpgateConfiguration> probeConfiguration)
    {
        _apiProxyService = apiProxyService;
        _warpgateConfiguration = probeConfiguration.Value;
    }

    public async Task<WarpgatePatternConfig> GetServerConfig()
    {
        if (_warpgateServerConfig == null)
        {
            var serverConfigString = await _apiProxyService.Get(new AiurUrl(_warpgateConfiguration.Instance, true));
            _warpgateServerConfig = JsonConvert.DeserializeObject<WarpgatePatternConfig>(serverConfigString);
        }

        return _warpgateServerConfig;
    }
}

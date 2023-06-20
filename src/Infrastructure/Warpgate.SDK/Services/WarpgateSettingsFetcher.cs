using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.Warpgate.SDK.Configuration;
using Aiursoft.Warpgate.SDK.Models.ViewModels;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.Warpgate.SDK.Services;

public class WarpgateSettingsFetcher : IScopedDependency
{
    private readonly CacheService _cacheService;
    private readonly AiurProtocolClient  _AiurProtocolClient ;
    private readonly WarpgateConfiguration _warpgateConfiguration;

    public WarpgateSettingsFetcher(
        CacheService cacheService,
        AiurProtocolClient  AiurProtocolClient ,
        IOptions<WarpgateConfiguration> probeConfiguration)
    {
        _cacheService = cacheService;
        _AiurProtocolClient  = AiurProtocolClient ;
        _warpgateConfiguration = probeConfiguration.Value;
    }

    public Task<WarpgatePatternConfig> GetServerConfig()
    {
        return _cacheService.RunWithCache("warpgate-server-config", async () =>
        {
            var serverConfigString = await _AiurProtocolClient .Get(new AiurApiEndpoint(_warpgateConfiguration.Instance, true));
            return JsonConvert.DeserializeObject<WarpgatePatternConfig>(serverConfigString);
        });
    }
}

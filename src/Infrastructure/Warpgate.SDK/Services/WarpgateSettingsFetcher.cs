using Aiursoft.Scanner.Abstractions;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.Canon;
using Aiursoft.Warpgate.SDK.Configuration;
using Aiursoft.Warpgate.SDK.Models.ViewModels;

namespace Aiursoft.Warpgate.SDK.Services;

public class WarpgateSettingsFetcher : IScopedDependency
{
    private readonly CacheService _cacheService;
    private readonly AiurProtocolClient _aiurProtocolClient;
    private readonly WarpgateConfiguration _warpgateConfiguration;

    public WarpgateSettingsFetcher(
        CacheService cacheService,
        AiurProtocolClient aiurProtocolClient,
        IOptions<WarpgateConfiguration> probeConfiguration)
    {
        _cacheService = cacheService;
        _aiurProtocolClient = aiurProtocolClient;
        _warpgateConfiguration = probeConfiguration.Value;
    }

    public Task<WarpgatePatternConfig> GetServerConfig()
    {
        return _cacheService.RunWithCache("warpgate-server-config",
            () =>
            {
                return _aiurProtocolClient.Get<WarpgatePatternConfig>(new AiurApiEndpoint(
                    _warpgateConfiguration.Instance,
                    "/",
                    new { }));
            });
    }
}
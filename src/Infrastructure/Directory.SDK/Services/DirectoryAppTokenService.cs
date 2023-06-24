using System;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Directory.SDK.Services.ToDirectoryServer;
using Aiursoft.Scanner.Abstraction;
using Microsoft.Extensions.Options;

namespace Aiursoft.Directory.SDK.Services;

/// <summary>
///     For saving other apps with appid and appsecret for current app.
/// </summary>
public class DirectoryAppTokenService : IScopedDependency
{
    private readonly AppsService _appsService;
    private readonly CacheService _cache;
    private readonly string _appId;
    private readonly string _appSecret;

    public DirectoryAppTokenService(
        AppsService appsService,
        CacheService cache,
        IOptions<DirectoryConfiguration> configuration)
    {
        _cache = cache;
        _appsService = appsService;
        _appId = configuration.Value.AppId;
        _appSecret = configuration.Value.AppSecret;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return await GetAccessTokenWithAppInfoAsync(_appId, _appSecret);
    }

    public Task<string> GetAccessTokenWithAppInfoAsync(string appId, string appSecret)
    {
        return _cache.QueryCacheWithSelector(
            cacheKey: $"access-token-{appId}",
            fallback: () => _appsService.AccessTokenAsync(appId, appSecret),
            selector: token => token.AccessToken,
            cachedMinutes: response => 
                response.DeadTime - DateTime.UtcNow - TimeSpan.FromSeconds(10));
    }
}
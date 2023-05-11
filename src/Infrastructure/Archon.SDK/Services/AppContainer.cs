using System;
using System.Threading.Tasks;
using Aiursoft.Archon.SDK.Services.ToArchonServer;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Archon.SDK.Services;

public class AppContainer
{
    private readonly string _appSecret;
    public readonly string AppId;
    private DateTime _accessTokenDeadTime;
    private string _latestAccessToken;

    public AppContainer(string appId, string appSecret)
    {
        AppId = appId;
        _appSecret = appSecret;
        _accessTokenDeadTime = DateTime.MinValue;
        _latestAccessToken = string.Empty;
    }

    public async Task<string> AccessToken(IServiceScopeFactory scopeFactory)
    {
        if (DateTime.UtcNow <= _accessTokenDeadTime) return _latestAccessToken;

        using var scope = scopeFactory.CreateScope();
        var archonApiService = scope.ServiceProvider.GetRequiredService<ArchonApiService>();
        var serverResult = await archonApiService.AccessTokenAsync(AppId, _appSecret);
        _latestAccessToken = serverResult.AccessToken;
        _accessTokenDeadTime = serverResult.DeadTime - TimeSpan.FromSeconds(20);
        return _latestAccessToken;
    }
}
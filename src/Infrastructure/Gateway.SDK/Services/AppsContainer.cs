using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Gateway.SDK.Services;

/// <summary>
///     For storaging other apps with appid and appsecret for current app.
/// </summary>
public class AppsContainer : ISingletonDependency
{
    private readonly List<AppContainer> _allApps;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _appId;
    private readonly string _appSecret;

    public AppsContainer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _allApps = new List<AppContainer>();
        _scopeFactory = scopeFactory;
        _appId = configuration["AiursoftAppId"];
        _appSecret = configuration["AiursoftAppSecret"];
    }

    public async Task<string> AccessTokenAsync()
    {
        return await AccessTokenAsync(_appId, _appSecret);
    }

    public async Task<string> AccessTokenAsync(string appId, string appSecret)
    {
        var app = GetApp(appId, appSecret);
        return await app.AccessToken(_scopeFactory);
    }

    private AppContainer GetApp(string appId, string appSecret)
    {
        var exists = _allApps.FirstOrDefault(t => t.AppId == appId);
        if (exists != null)
        {
            return exists;
        }

        var newContainer = new AppContainer(appId, appSecret);
        _allApps.Add(newContainer);
        exists = newContainer;
        return exists;
    }
}
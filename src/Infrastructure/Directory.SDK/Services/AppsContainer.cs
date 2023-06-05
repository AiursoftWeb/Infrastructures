using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Scanner.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aiursoft.Directory.SDK.Services;

/// <summary>
///     For saving other apps with appid and appsecret for current app.
/// </summary>
public class AppsContainer : ISingletonDependency
{
    private readonly HashSet<AppContainer> _allApps = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _appId;
    private readonly string _appSecret;

    public AppsContainer(
        IOptions<DirectoryConfiguration> configuration,
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _appId = configuration.Value.AppId;
        _appSecret = configuration.Value.AppSecret;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return await GetAccessTokenAsyncWithAppInfo(_appId, _appSecret);
    }

    public async Task<string> GetAccessTokenAsyncWithAppInfo(string appId, string appSecret)
    {
        var app = GetApp(appId, appSecret);
        return await app.GetCachedAccessTokenAsync(_scopeFactory);
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
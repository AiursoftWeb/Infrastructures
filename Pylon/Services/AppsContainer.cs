using Aiursoft.Pylon.Services.ToArchonServer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    /// <summary>
    /// For storaging other apps with appid and appsecret for current app.
    /// </summary>
    public class AppsContainer
    {
        private readonly List<AppContainer> _allApps;
        private readonly IServiceScopeFactory _scopeFactory;

        public AppsContainer(IServiceScopeFactory scopeFactory)
        {
            _allApps = new List<AppContainer>();
            _scopeFactory = scopeFactory;
        }

        private AppContainer GetApp(string appId, string appSecret)
        {
            var exists = _allApps.FirstOrDefault(t => t.AppId == appId);
            if (exists == null)
            {
                var newContainer = new AppContainer(appId, appSecret);
                _allApps.Add(newContainer);
                exists = newContainer;
            }
            return exists;
        }

        public async Task<string> AccessToken()
        {
            return await AccessToken(Extends.CurrentAppId, Extends.CurrentAppSecret);
        }

        public async Task<string> AccessToken(string appId, string appSecret)
        {
            var app = GetApp(appId, appSecret);
            return await app.AccessToken(_scopeFactory);
        }
    }

    public class AppContainer
    {
        public readonly string AppId;
        private readonly string _appSecret;
        public AppContainer(string appId, string appSecret)
        {
            AppId = appId;
            _appSecret = appSecret;
        }

        public async Task<string> AccessToken(IServiceScopeFactory scopeFactory)
        {
            if (DateTime.UtcNow > _accessTokenDeadTime)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var archonApiService = scope.ServiceProvider.GetRequiredService<ArchonApiService>();
                    var serverResult = await archonApiService.AccessTokenAsync(AppId, _appSecret);
                    _latestAccessToken = serverResult.AccessToken;
                    _accessTokenDeadTime = serverResult.DeadTime - TimeSpan.FromSeconds(20);
                }
            }
            return _latestAccessToken;
        }
        private string _latestAccessToken { get; set; } = string.Empty;
        private DateTime _accessTokenDeadTime { get; set; } = DateTime.MinValue;
    }
}
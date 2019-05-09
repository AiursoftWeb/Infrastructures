using Aiursoft.Pylon.Services.ToAPIServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Aiursoft.Pylon.Services.ToArchonServer;

namespace Aiursoft.Pylon.Services
{
    /// <summary>
    /// For storaging other apps with appid and appsecret for current app.
    /// </summary>
    public class AppsContainer
    {
        private List<AppContainer> _allApps;
        private IServiceScopeFactory _scopeFactory;

        public AppsContainer(IServiceScopeFactory scopeFactory)
        {
            if (_allApps == null)
            {
                _allApps = new List<AppContainer>();
            }
            else
            {
                throw new InvalidOperationException("Created two all apps but this is singlton design patten!");
            }
            _scopeFactory = scopeFactory;
        }
        private AppContainer GetApp(string appId, string appSecret)
        {
            var exists = _allApps.Find(t => t.CurrentAppId == appId);
            if (exists == null)
            {
                var newContainer = new AppContainer(appId, appSecret);
                _allApps.Add(newContainer);
                exists = _allApps.Find(t => t.CurrentAppId == appId);
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

    class AppContainer
    {
        public AppContainer(string appId, string appSecret)
        {
            CurrentAppId = appId;
            CurrentAppSecret = appSecret;
        }
        public string CurrentAppId { get; private set; } = string.Empty;
        public string CurrentAppSecret { get; private set; } = string.Empty;
        public async Task<string> AccessToken(IServiceScopeFactory scopeFactory)
        {
            if (DateTime.UtcNow > _accessTokenDeadTime)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var archonApiService = scope.ServiceProvider.GetRequiredService<ArchonApiService>();
                    var ServerResult = await archonApiService.AccessTokenAsync(CurrentAppId, CurrentAppSecret);
                    _latestAccessToken = ServerResult.AccessToken;
                    _accessTokenDeadTime = ServerResult.DeadTime;
                }
            }
            return _latestAccessToken;
        }
        private string _latestAccessToken { get; set; } = string.Empty;
        private DateTime _accessTokenDeadTime { get; set; } = DateTime.MinValue;
    }
}
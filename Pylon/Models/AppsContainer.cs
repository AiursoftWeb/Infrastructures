using Aiursoft.Pylon.Services.ToAPIServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace Aiursoft.Pylon.Models
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
        private AppContainer GetApp(string AppId, string AppSecret)
        {
            var exists = _allApps.Find(t => t.CurrentAppId == AppId);
            if (exists == null)
            {
                var newContainer = new AppContainer(AppId, AppSecret);
                _allApps.Add(newContainer);
                exists = _allApps.Find(t => t.CurrentAppId == AppId);
            }
            return exists;
        }

        public async Task<string> AccessToken()
        {
            return await AccessToken(Extends.CurrentAppId, Extends.CurrentAppSecret);
        }

        public async Task<string> AccessToken(string AppId, string AppSecret)
        {
            var app = GetApp(AppId, AppSecret);
            return await app.AccessToken(_scopeFactory);
        }
    }
    class AppContainer
    {
        public AppContainer(string AppId, string AppSecret)
        {
            CurrentAppId = AppId;
            CurrentAppSecret = AppSecret;
        }
        public string CurrentAppId { get; private set; } = string.Empty;
        public string CurrentAppSecret { get; private set; } = string.Empty;
        public async Task<string> AccessToken(IServiceScopeFactory _scopeFactory)
        {
            if (DateTime.Now > _accessTokenDeadTime)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var coreApiService = scope.ServiceProvider.GetRequiredService<CoreApiService>();
                    var ServerResult = await coreApiService.AccessTokenAsync(CurrentAppId, CurrentAppSecret);
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
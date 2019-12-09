using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Services.ToArchonServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    /// <summary>
    /// For storaging other apps with appid and appsecret for current app.
    /// </summary>
    public class AppsContainer : ISingletonDependency
    {
        public static string CurrentAppName { get; set; }
        public readonly string _currentAppId;
        private readonly string _currentAppSecret;
        private readonly List<AppContainer> _allApps;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AppsContainer> _logger;

        public AppsContainer(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<AppsContainer> logger)
        {
            _allApps = new List<AppContainer>();
            _scopeFactory = scopeFactory;
            _logger = logger;
            _currentAppId = configuration[$"{CurrentAppName}AppId"];
            _currentAppSecret = configuration[$"{CurrentAppName}AppSecret"];
            if (string.IsNullOrWhiteSpace(_currentAppId) || string.IsNullOrWhiteSpace(_currentAppSecret))
            {
                _logger.LogError("Did not get appId and appSecret from configuration!");
            }
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
            return await AccessToken(_currentAppId, _currentAppSecret);
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
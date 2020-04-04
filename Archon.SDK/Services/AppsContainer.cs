using Aiursoft.Scanner.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Archon.SDK.Services
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

        public AppsContainer(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _allApps = new List<AppContainer>();
            _scopeFactory = scopeFactory;
            _currentAppId = configuration[$"{CurrentAppName}AppId"];
            _currentAppSecret = configuration[$"{CurrentAppName}AppSecret"];
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

        private AppContainer GetApp(string appId, string appSecret)
        {
            var exists = _allApps.FirstOrDefault(t => t._appId == appId);
            if (exists == null)
            {
                var newContainer = new AppContainer(appId, appSecret);
                _allApps.Add(newContainer);
                exists = newContainer;
            }
            return exists;
        }
    }
}
using Aiursoft.Pylon.Services.ToAPIServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models
{
    /// <summary>
    /// For storaging other apps with appid and appsecret for current app.
    /// </summary>
    public static class AppsContainer
    {
        private static List<AppContainer> AllApps { get; set; }
        static AppsContainer()
        {
            AllApps = new List<AppContainer>();
        }
        private static AppContainer GetApp(string AppId, string AppSecret)
        {
            var exists = AllApps.Find(t => t.CurrentAppId == AppId);
            if (exists == null)
            {
                var newContainer = new AppContainer(AppId, AppSecret);
                AllApps.Add(newContainer);
                exists = AllApps.Find(t => t.CurrentAppId == AppId);
            }
            return exists;
        }

        public static Func<Task<string>> AccessToken()
        {
            return AccessToken(Extends.CurrentAppId, Extends.CurrentAppSecret);
        }

        public static Func<Task<string>> AccessToken(string AppId, string AppSecret)
        {
            var app = GetApp(AppId, AppSecret);
            return app.AccessToken;
        }
    }
    class AppContainer
    {
        public AppContainer(string AppId, string AppSecret)
        {
            this.CurrentAppId = AppId;
            this.CurrentAppSecret = AppSecret;
        }
        public string CurrentAppId { get; private set; } = string.Empty;
        public string CurrentAppSecret { get; private set; } = string.Empty;
        public async Task<string> AccessToken()
        {
            if (DateTime.Now > _accessTokenDeadTime)
            {
                var ServerResult = await ApiService.AccessTokenAsync(CurrentAppId, CurrentAppSecret);
                _latestAccessToken = ServerResult.AccessToken;
                _accessTokenDeadTime = ServerResult.DeadTime;
            }
            return _latestAccessToken;
        }
        private string _latestAccessToken { get; set; } = string.Empty;
        private DateTime _accessTokenDeadTime { get; set; } = DateTime.MinValue;
    }
}
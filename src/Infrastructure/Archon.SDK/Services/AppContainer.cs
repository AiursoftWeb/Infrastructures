using Aiursoft.Archon.SDK.Services.ToArchonServer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Aiursoft.Archon.SDK.Services
{
    public class AppContainer
    {
        public readonly string AppId;
        private readonly string _appSecret;
        private string _latestAccessToken;
        private DateTime _accessTokenDeadTime;

        public AppContainer(string appId, string appSecret)
        {
            AppId = appId;
            _appSecret = appSecret;
            _accessTokenDeadTime = DateTime.MinValue;
            _latestAccessToken = string.Empty;
        }

        public async Task<string> AccessToken(IServiceScopeFactory scopeFactory)
        {
            if (DateTime.UtcNow <= _accessTokenDeadTime)
            {
                return _latestAccessToken;
            }

            using IServiceScope scope = scopeFactory.CreateScope();
            var archonApiService = scope.ServiceProvider.GetRequiredService<ArchonApiService>();
            var serverResult = await archonApiService.AccessTokenAsync(AppId, _appSecret);
            _latestAccessToken = serverResult.AccessToken;
            _accessTokenDeadTime = serverResult.DeadTime - TimeSpan.FromSeconds(20);
            return _latestAccessToken;
        }
    }
}

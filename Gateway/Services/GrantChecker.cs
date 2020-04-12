using Aiursoft.Archon.SDK.Services;
using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Models.Developer;
using Aiursoft.SDK.Services.ToDeveloperServer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class GrantChecker : IScopedDependency
    {
        private readonly GatewayDbContext _dbContext;
        private readonly DeveloperApiService _developerApiService;
        private readonly ACTokenManager _tokenManager;
        private readonly AiurCache _aiurCache;

        public GrantChecker(
            GatewayDbContext context,
            DeveloperApiService developerApiService,
            ACTokenManager tokenManager,
            AiurCache aiurCache)
        {
            _dbContext = context;
            _developerApiService = developerApiService;
            _tokenManager = tokenManager;
            _aiurCache = aiurCache;
        }

        public async Task<GatewayUser> EnsureGranted(string accessToken, string userId, Func<App, bool> prefix)
        {
            var appid = await _tokenManager.ValidateAccessToken(accessToken);
            var targetUser = await _dbContext.Users.Include(t => t.Emails).SingleOrDefaultAsync(t => t.Id == userId);
            var app = await _aiurCache.GetAndCache($"app-info-cache-{appid}", () => _developerApiService.AppInfoAsync(appid));
            if (!_dbContext.LocalAppGrant.Any(t => t.AppID == appid && t.GatewayUserId == targetUser.Id))
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "This user did not grant your app!");
            }
            if (prefix != null && !prefix(app.App))
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "You app is not allowed to do that!");
            }
            return targetUser;
        }
    }
}
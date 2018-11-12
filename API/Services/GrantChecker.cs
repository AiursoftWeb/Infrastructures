using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aiursoft.API.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Aiursoft.Pylon.Services;
using Microsoft.Extensions.DependencyInjection;
using Aiursoft.API.Models;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Models.Developer;

namespace Aiursoft.API.Services
{

    public class GrantChecker
    {
        private readonly APIDbContext _dbContext;
        private readonly DeveloperApiService _developerApiService;
        private readonly ACTokenManager _tokenManager;

        public GrantChecker(
            APIDbContext context,
            DeveloperApiService developerApiService,
            ACTokenManager tokenManager)
        {
            _dbContext = context;
            _developerApiService = developerApiService;
            _tokenManager = tokenManager;
        }

        public async Task<APIUser> EnsureGranted(string accessToken, string userId, Func<App, bool> prefix)
        {
            var appid = _tokenManager.ValidateAccessToken(accessToken);
            var targetUser = await _dbContext.Users.FindAsync(userId);
            var app = await _developerApiService.AppInfoAsync(appid);
            if (!_dbContext.LocalAppGrant.Any(t => t.AppID == appid && t.APIUserId == targetUser.Id))
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
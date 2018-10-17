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

        public GrantChecker(
            APIDbContext _context,
            DeveloperApiService developerApiService)
        {
            _dbContext = _context;
            _developerApiService = developerApiService;
        }

        public async Task<APIUser> EnsureGranted(string accessToken, string userId, Func<App, bool> prefix)
        {
            var token = await _dbContext
                .AccessToken
                .SingleOrDefaultAsync(t => t.Value == accessToken);
            if (token == null)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "We can not validate your access token!");
            }
            if (!token.IsAlive)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Your access token is already Timeout!");
            }
            var targetUser = await _dbContext.Users.FindAsync(userId);
            var app = await _developerApiService.AppInfoAsync(token.ApplyAppId);
            if (app.Code != ErrorType.Success)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "Can not find your app with your accesstoken!");
            }
            if (!_dbContext.LocalAppGrant.Any(t => t.AppID == token.ApplyAppId && t.APIUserId == targetUser.Id))
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
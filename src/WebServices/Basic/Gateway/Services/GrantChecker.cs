using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Developer.SDK.Models;
using Aiursoft.Developer.SDK.Services.ToDeveloperServer;
using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Gateway.Services;

public class GrantChecker : IScopedDependency
{
    private readonly GatewayDbContext _dbContext;
    private readonly DeveloperApiService _developerApiService;
    private readonly ACTokenValidator _tokenManager;

    public GrantChecker(
        GatewayDbContext context,
        DeveloperApiService developerApiService,
        ACTokenValidator tokenManager)
    {
        _dbContext = context;
        _developerApiService = developerApiService;
        _tokenManager = tokenManager;
    }

    public async Task<GatewayUser> EnsureGranted(string accessToken, string userId, Func<App, bool> prefix)
    {
        var appid = _tokenManager.ValidateAccessToken(accessToken);
        var targetUser = await _dbContext.Users.Include(t => t.Emails).SingleOrDefaultAsync(t => t.Id == userId);
        var app = await _developerApiService.AppInfoAsync(appid);
        if (!_dbContext.LocalAppGrant.Any(t => t.AppId == appid && t.GatewayUserId == targetUser.Id))
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
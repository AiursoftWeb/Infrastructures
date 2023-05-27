using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Developer.SDK.Models;
using Aiursoft.Developer.SDK.Services.ToDeveloperServer;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.Models;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Directory.Services;

public class GrantChecker : IScopedDependency
{
    private readonly DirectoryDbContext _dbContext;
    private readonly DeveloperApiService _developerApiService;
    private readonly AiursoftAppTokenValidator _tokenManager;

    public GrantChecker(
        DirectoryDbContext context,
        DeveloperApiService developerApiService,
        AiursoftAppTokenValidator tokenManager)
    {
        _dbContext = context;
        _developerApiService = developerApiService;
        _tokenManager = tokenManager;
    }

    public async Task<DirectoryUser> EnsureGranted(string accessToken, string userId, Func<App, bool> prefix)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(accessToken);
        var targetUser = await _dbContext.Users.Include(t => t.Emails).SingleOrDefaultAsync(t => t.Id == userId);
        var app = await _developerApiService.AppInfoAsync(appid);
        if (!_dbContext.LocalAppGrant.Any(t => t.AppId == appid && t.DirectoryUserId == targetUser.Id))
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
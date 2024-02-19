using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.Models;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Scanner.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Directory.Services;

public class GrantChecker : IScopedDependency
{
    private readonly DirectoryDbContext _dbContext;
    private readonly AiursoftAppTokenValidator _tokenManager;

    public GrantChecker(
        DirectoryDbContext context,
        AiursoftAppTokenValidator tokenManager)
    {
        _dbContext = context;
        _tokenManager = tokenManager;
    }

    public async Task<DirectoryUser> EnsureGranted(string accessToken, string userId, Func<DirectoryApp, bool> prefix)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(accessToken);
        var targetUser = await _dbContext.Users.Include(t => t.Emails).FirstOrDefaultAsync(t => t.Id == userId);
        if (targetUser == null)
        {
            throw new AiurServerException(Code.NotFound, $"The user with ID: {userId} was not found!");
        }
        
        var app = await _dbContext.DirectoryAppsInDb.FindAsync(appid);
        if (app == null)
        {
            throw new AiurServerException(Code.NotFound, $"The app with ID: {appid} was not found!");
        }
        
        if (!_dbContext.LocalAppGrant.Any(t => t.AppId == appid && t.DirectoryUserId == targetUser.Id))
        {
            throw new AiurServerException(Code.Unauthorized, "This user did not grant your app!");
        }

        if (prefix != null && !prefix(app))
        {
            throw new AiurServerException(Code.Unauthorized, "You app is not allowed to do that!");
        }

        return targetUser;
    }
}

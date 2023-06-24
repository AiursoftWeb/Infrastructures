using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.Directory.Controllers;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.Models;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.ForApps.AddressModels;
using Aiursoft.Scanner.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Directory.Services;

public class UserAppAuthManager : IScopedDependency
{
    private readonly DirectoryDbContext _dbContext;

    public UserAppAuthManager(DirectoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> FinishAuth(DirectoryUser user, FinishAuthInfo model, bool forceGrant, bool trusted)
    {
        var authorized = await HasAuthorizedApp(user, model.AppId);
        if (!authorized && trusted)
        {
            // Unauthorized. But viewing a trusted app. Just auto auth him.
            await GrantTargetApp(user, model.AppId);
            authorized = true;
        }

        if (authorized && forceGrant != true)
        {
            // Dont need to auth, and the user don't force to auth.
            var pack = await GeneratePack(user, model.AppId);
            var url = new AiurApiEndpoint(GetRegexRedirectUri(model.RedirectUri), new AuthResultAddressModel
            {
                Code = pack.Code,
                State = model.State
            });
            return new RedirectResult(url.ToString());
        }
        else
        {
            // Need to do the auth logic.
            var url = new AiurApiEndpoint(string.Empty, "OAuth", nameof(OAuthController.AuthorizeConfirm), new FinishAuthInfo
            {
                AppId = model.AppId,
                RedirectUri = model.RedirectUri,
                State = model.State
            });
            return new RedirectResult(url.ToString());
        }
    }

    public async Task GrantTargetApp(DirectoryUser user, string appId)
    {
        if (!await HasAuthorizedApp(user, appId))
        {
            var appGrant = new AppGrant
            {
                AppId = appId,
                DirectoryUserId = user.Id
            };
            await _dbContext.LocalAppGrant.AddAsync(appGrant);
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task<OAuthPack> GeneratePack(DirectoryUser user, string appId)
    {
        var pack = new OAuthPack
        {
            Code = Math.Abs(Guid.NewGuid().GetHashCode()),
            UserId = user.Id,
            ApplyAppId = appId
        };
        await _dbContext.OAuthPack.AddAsync(pack);
        await _dbContext.SaveChangesAsync();
        return pack;
    }

    public Task<bool> HasAuthorizedApp(DirectoryUser user, string appId)
    {
        return _dbContext
            .LocalAppGrant
            .Where(t => t.AppId == appId)
            .AnyAsync(t => t.DirectoryUserId == user.Id);
    }

    private string GetRegexRedirectUri(string sourceUrl)
    {
        var url = new Uri(sourceUrl);
        return $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
    }
}
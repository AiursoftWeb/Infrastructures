﻿using Aiursoft.AiurProtocol.Models;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.AiurProtocol.Server.Attributes;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.SDK.Models.API.AccountAddressModels;
using Aiursoft.Directory.SDK.Models.API.AccountViewModels;
using Aiursoft.Directory.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Directory.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class AccountController : ControllerBase
{
    private readonly UserAppAuthManager _authManager;
    private readonly DirectoryDbContext _dbContext;
    private readonly AiursoftAppTokenValidator _tokenManager;

    public AccountController(
        AiursoftAppTokenValidator tokenManager,
        DirectoryDbContext dbContext,
        UserAppAuthManager authManager)
    {
        _tokenManager = tokenManager;
        _dbContext = dbContext;
        _authManager = authManager;
    }

    [Produces(typeof(CodeToOpenIdViewModel))]
    public async Task<IActionResult> CodeToOpenId(CodeToOpenIdAddressModel model)
    {
        var appId =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var targetPack = await _dbContext
            .OAuthPack
            .SingleOrDefaultAsync(t => t.Code == model.Code);

        if (targetPack == null)
        {
            return this.Protocol(Code.WrongKey, "The code doesn't exists in our database.");
        }

        // Use time is more than 10 seconds from now.
        if (targetPack.UseTime != DateTime.MinValue && targetPack.UseTime + TimeSpan.FromSeconds(10) < DateTime.UtcNow)
        {
            return this.Protocol(Code.Unauthorized, "Code is used already!");
        }

        if (targetPack.ApplyAppId != appId)
        {
            return this.Protocol(Code.Unauthorized, "The app granted code is not the app granting access token!");
        }

        var currentApp = await _dbContext.DirectoryAppsInDb.FindAsync(appId);
        if (currentApp == null)
        {
            return this.Protocol(Code.NotFound, $"The app with ID: '{appId}' was not found!.");
        }
        
        if (!currentApp.ViewOpenId)
        {
            return this.Protocol(Code.Unauthorized, "The app doesn't have view open id permission.");
        }

        targetPack.UseTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        var viewModel = new CodeToOpenIdViewModel
        {
            OpenId = targetPack.UserId,
            Scope = "scope",
            Message = "Successfully get user openid",
            Code = Code.ResultShown
        };
        return this.Protocol(viewModel);
    }

    [Produces(typeof(UserInfoViewModel))]
    public async Task<IActionResult> UserInfo(UserInfoAddressModel model)
    {
        var appId =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var user = await _dbContext.Users.Include(t => t.Emails).SingleOrDefaultAsync(t => t.Id == model.OpenId);
        if (user == null)
        {
            return this.Protocol(Code.NotFound, "Can not find a user with open id: " + model.OpenId);
        }

        if (!await _authManager.HasAuthorizedApp(user, appId))
        {
            return this.Protocol(Code.Unauthorized,
                "The user did not allow your app to view his personal info! App Id: " + model.OpenId);
        }

        var viewModel = new UserInfoViewModel
        {
            Code = 0,
            Message = "Successfully get target user info.",
            User = user
        };
        return this.Protocol(viewModel);
    }
}
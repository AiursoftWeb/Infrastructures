using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.DBTools;
using Aiursoft.DBTools.Models;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Directory.SDK.Models.API.AppsAddressModels;
using Aiursoft.Directory.SDK.Models.API.AppsViewModels;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Directory.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Directory.Controllers;

[APIRemoteExceptionHandler]
[APIModelStateChecker]
[LimitPerMin]
public class AppsController : ControllerBase
{
    private readonly TokenGenerator _tokenGenerator;
    private readonly AiursoftAppTokenValidator _tokenManager;
    private readonly DirectoryDbContext _dbContext;

    public AppsController(
        TokenGenerator tokenGenerator,
        AiursoftAppTokenValidator tokenManager,
        DirectoryDbContext context)
    {
        _tokenGenerator = tokenGenerator;
        _tokenManager = tokenManager;
        _dbContext = context;
    }
    
    public async Task<IActionResult> IsValidApp(IsValidateAppAddressModel model)
    {
        var target = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (target == null)
        {
            return this.Protocol(new AiurProtocol { Message = "Target app did not found.", Code = ErrorType.NotFound });
        }

        if (target.AppSecret != model.AppSecret)
        {
            return this.Protocol(new AiurProtocol { Message = "Wrong secret.", Code = ErrorType.WrongKey });
        }

        return this.Protocol(new AiurProtocol { Message = "Correct app info.", Code = ErrorType.Success });
    }

    [Produces(typeof(AppInfoViewModel))]
    public async Task<IActionResult> AppInfo(AppInfoAddressModel model)
    {
        var target = await _dbContext
            .DirectoryAppsInDb
            .FindAsync(model.AppId);

        if (target == null)
        {
            return this.Protocol(new AiurProtocol
                { Message = $"Could find target app with appId: '{model.AppId}'!", Code = ErrorType.NotFound });
        }

        return this.Protocol(new AppInfoViewModel
        {
            Message = "Successfully get target app info.",
            Code = ErrorType.Success,
            App = target
        });
    }
    
    [Produces(typeof(AccessTokenViewModel))]
    public async Task<IActionResult> AccessToken(AccessTokenAddressModel model)
    {
        var target = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (target == null)
        {
            return this.Protocol(new AiurProtocol { Message = "Target app did not found.", Code = ErrorType.NotFound });
        }

        if (target.AppSecret != model.AppSecret)
        {
            return this.Protocol(new AiurProtocol { Message = "Wrong secret.", Code = ErrorType.WrongKey });
        }
        
        var token = _tokenGenerator.GenerateAccessToken(model.AppId);
        return this.Protocol(new AccessTokenViewModel
        {
            Code = ErrorType.Success,
            Message = "Successfully get access token.",
            AccessToken = token.tokenString,
            DeadTime = token.expireTime
        });
    }
    
    [APIRemoteExceptionHandler]
    [APIModelStateChecker]
    [Produces(typeof(AiurPagedCollection<Grant>))]
    public async Task<IActionResult> AllUserGranted(AllUserGrantedAddressModel model)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var query = _dbContext
            .LocalAppGrant
            .Include(t => t.User)
            .Where(t => t.AppId == appid)
            .OrderByDescending(t => t.GrantTime);
        var result = await AiurPagedCollectionBuilder.BuildAsync<Grant>(
            query,
            model,
            ErrorType.Success,
            "Successfully get all your users");
        return this.Protocol(result);
    }

    [HttpPost]
    [APIRemoteExceptionHandler]
    [APIModelStateChecker]
    public async Task<IActionResult> DropGrants([Required] string accessToken)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(accessToken);
        _dbContext.LocalAppGrant.Delete(t => t.AppId == appid);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(ErrorType.Success, "Successfully dropped all users granted!");
    }
}
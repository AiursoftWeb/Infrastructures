using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.DBTools;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Directory.SDK.Models.API.AppsAddressModels;
using Aiursoft.Directory.SDK.Models.API.AppsViewModels;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Directory.Services;
using Aiursoft.AiurProtocol.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
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
            return this.Protocol(new AiurResponse { Message = "Target app did not found.", Code = Code.NotFound });
        }

        if (target.AppSecret != model.AppSecret)
        {
            return this.Protocol(new AiurResponse { Message = "Wrong secret.", Code = Code.WrongKey });
        }

        return this.Protocol(new AiurResponse { Message = "Correct app info.", Code = Code.NoActionTaken });
    }

    [Produces(typeof(AppInfoViewModel))]
    public async Task<IActionResult> AppInfo(AppInfoAddressModel model)
    {
        var target = await _dbContext
            .DirectoryAppsInDb
            .FindAsync(model.AppId);

        if (target == null)
        {
            return this.Protocol(new AiurResponse
            { Message = $"Could find target app with appId: '{model.AppId}'!", Code = Code.NotFound });
        }

        return this.Protocol(new AppInfoViewModel
        {
            Message = "Successfully get target app info.",
            Code = Code.ResultShown,
            App = target
        });
    }

    [Produces(typeof(AccessTokenViewModel))]
    public async Task<IActionResult> AccessToken(AccessTokenAddressModel model)
    {
        var target = await _dbContext.DirectoryAppsInDb.FindAsync(model.AppId);
        if (target == null)
        {
            return this.Protocol(new AiurResponse { Message = "Target app did not found.", Code = Code.NotFound });
        }

        if (target.AppSecret != model.AppSecret)
        {
            return this.Protocol(new AiurResponse { Message = "Wrong secret.", Code = Code.WrongKey });
        }

        var token = _tokenGenerator.GenerateAccessToken(model.AppId);
        return this.Protocol(new AccessTokenViewModel
        {
            Code = Code.ResultShown,
            Message = "Successfully get access token.",
            AccessToken = token.tokenString,
            DeadTime = token.expireTime
        });
    }

    [ApiExceptionHandler]
    [ApiModelStateChecker]
    [Produces(typeof(AiurPagedCollection<Grant>))]
    public async Task<IActionResult> AllUserGranted(AllUserGrantedAddressModel model)
    {
        var appid = await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var query = _dbContext
            .LocalAppGrant
            .Include(t => t.User)
            .Where(t => t.AppId == appid)
            .OrderByDescending(t => t.GrantTime);
        return await this.Protocol(Code.ResultShown, "Successfully get all your users", query, model);
    }

    [HttpPost]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> DropGrants([Required] string accessToken)
    {
        var appid = await _tokenManager.ValidateAccessTokenAsync(accessToken);
        _dbContext.LocalAppGrant.Delete(t => t.AppId == appid);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(Code.JobDone, "Successfully dropped all users granted!");
    }
}
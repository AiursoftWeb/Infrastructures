using System;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.Directory.SDK.Services;

using Aiursoft.AiurProtocol.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models.TokenAddressModels;
using Aiursoft.Probe.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Probe.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class TokenController : ControllerBase
{
    private readonly ProbeDbContext _dbContext;
    private readonly ProbeTokenManager _probeTokenManager;
    private readonly AiursoftAppTokenValidator _tokenManager;

    public TokenController(
        AiursoftAppTokenValidator tokenManager,
        ProbeDbContext dbContext,
        ProbeTokenManager probeTokenManager)
    {
        _tokenManager = tokenManager;
        _dbContext = dbContext;
        _probeTokenManager = probeTokenManager;
    }

    // TODO: Merge this with Directory token!
    [HttpPost]
    [Produces(typeof(AiurValue<string>))]
    public async Task<IActionResult> GetToken(GetTokenAddressModel model)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var site = await _dbContext
            .Sites
            .SingleOrDefaultAsync(t => t.SiteName == model.SiteName);
        if (site == null)
        {
            return this.Protocol(Code.NotFound, $"Could not find a site with name: '{model.SiteName}'");
        }

        if (site.AppId != appid)
        {
            return this.Protocol(Code.Unauthorized,
                $"The site '{model.SiteName}' you tried to get a PBToken is not your app's site.");
        }

        var (pbToken, deadline) = _probeTokenManager.GenerateAccessToken(site.SiteName, model.UnderPath, model.Permissions,
            TimeSpan.FromSeconds(model.LifespanSeconds));
        return this.Protocol(new AiurValue<string>(pbToken)
        {
            Code = Code.ResultShown,
            Message = $"Successfully get your PBToken! Use it before {deadline} UTC!"
        });
    }
}
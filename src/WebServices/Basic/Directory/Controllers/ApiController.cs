using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.DBTools;
using Aiursoft.DBTools.Models;
using Aiursoft.Developer.SDK.Services.ToDeveloperServer;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.SDK.Models.API;
using Aiursoft.Directory.SDK.Models.API.APIAddressModels;
using Aiursoft.Directory.SDK.Models.API.APIViewModels;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Directory.Services;
using Aiursoft.Directory.Models;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Directory.Models.ApiViewModels;

namespace Aiursoft.Directory.Controllers;

public class ApiController : Controller
{
    private readonly DirectoryDbContext _dbContext;
    private readonly AiursoftAppTokenValidator _tokenManager;
    private readonly UserManager<DirectoryUser> _userManager;
    private readonly DeveloperApiService _developerApiService;
    private readonly TokenGenerator _tokenGenerator;

    public ApiController(
        TokenGenerator tokenGenerator,
        DeveloperApiService developerApiService,
        UserManager<DirectoryUser> userManager,
        DirectoryDbContext context,
        AiursoftAppTokenValidator tokenManager)
    {
        _tokenGenerator = tokenGenerator;
        _developerApiService = developerApiService;
        _userManager = userManager;
        _dbContext = context;
        _tokenManager = tokenManager;
    }

    private void _ApplyCultureCookie(string culture)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true
            });
    }

    [HttpGet("set-language")]
    public IActionResult SetLang(SetLangAddressModel model)
    {
        return View(new SetLangViewModel
        {
            Host = model.Host,
            Path = model.Path
        });
    }

    [HttpPost("set-language")]
    public async Task<IActionResult> SetLang(SetLangViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            _ApplyCultureCookie(model.Culture);
        }
        catch (CultureNotFoundException)
        {
            return this.Protocol(new AiurProtocol { Message = "Not a language.", Code = ErrorType.InvalidInput });
        }

        var user = await GetCurrentUserAsync();
        if (user != null)
        {
            user.PreferedLanguage = model.Culture;
            await _userManager.UpdateAsync(user);
        }

        var toGo = new AiurUrl(model.Host, "/switch-language", new
        {
            model.Culture,
            ReturnUrl = model.Path
        });
        return Redirect(toGo.ToString());
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

    private Task<DirectoryUser> GetCurrentUserAsync()
    {
        return _dbContext
            .Users
            .Include(t => t.Emails)
            .FirstOrDefaultAsync(t => t.UserName == User.Identity.Name);
    }

    [APIRemoteExceptionHandler]
    [APIModelStateChecker]
    [Produces(typeof(AccessTokenViewModel))]
    public async Task<IActionResult> AccessToken(AccessTokenAddressModel model)
    {
        var correctApp = await _developerApiService.IsValidAppAsync(model.AppId, model.AppSecret);
        if (correctApp)
        {
            var token = _tokenGenerator.GenerateAccessToken(model.AppId);
            return this.Protocol(new AccessTokenViewModel
            {
                Code = ErrorType.Success,
                Message = "Successfully get access token.",
                AccessToken = token.tokenString,
                DeadTime = token.expireTime
            });
        }

        return this.Protocol(ErrorType.WrongKey, "Wrong app info.");
    }
}
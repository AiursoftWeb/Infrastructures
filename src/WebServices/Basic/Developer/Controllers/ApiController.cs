using System;
using System.Threading.Tasks;
using Aiursoft.Developer.Data;
using Aiursoft.Developer.SDK.Models.ApiAddressModels;
using Aiursoft.Developer.SDK.Models.ApiViewModels;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Developer.Controllers;

[APIExpHandler]
[APIModelStateChecker]
[LimitPerMin]
public class ApiController : ControllerBase
{
    private readonly DeveloperDbContext _dbContext;

    public ApiController(
        DeveloperDbContext context)
    {
        _dbContext = context;
    }

    public IActionResult Error()
    {
        throw new Exception("This is a test API error for debugging.");
    }

    public async Task<IActionResult> IsValidApp(IsValidateAppAddressModel model)
    {
        var target = await _dbContext.Apps.FindAsync(model.AppId);
        if (target == null)
            return this.Protocol(new AiurProtocol { Message = "Target app did not found.", Code = ErrorType.NotFound });
        if (target.AppSecret != model.AppSecret)
            return this.Protocol(new AiurProtocol { Message = "Wrong secret.", Code = ErrorType.WrongKey });
        return this.Protocol(new AiurProtocol { Message = "Correct app info.", Code = ErrorType.Success });
    }

    [Produces(typeof(AppInfoViewModel))]
    public async Task<IActionResult> AppInfo(AppInfoAddressModel model)
    {
        var target = await _dbContext
            .Apps
            .SingleOrDefaultAsync(t => t.AppId == model.AppId);

        if (target == null)
            return this.Protocol(new AiurProtocol
                { Message = $"Could find target app with appId: '{model.AppId}'!", Code = ErrorType.NotFound });
        return this.Protocol(new AppInfoViewModel
        {
            Message = "Successfully get target app info.",
            Code = ErrorType.Success,
            App = target
        });
    }
}
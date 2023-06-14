using System.Threading.Tasks;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.SDK.Models.API.AppsAddressModels;
using Aiursoft.Directory.SDK.Models.API.AppsViewModels;
using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Directory.Controllers;

public class AppsController : ControllerBase
{
    private readonly DirectoryDbContext _dbContext;

    public AppsController(
        DirectoryDbContext context)
    {
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
}
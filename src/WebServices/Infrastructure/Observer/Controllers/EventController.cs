using System.Linq;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Canon;
using Aiursoft.DBTools;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Observer.Data;
using Aiursoft.Observer.SDK.Models;
using Aiursoft.Observer.SDK.Models.EventAddressModels;
using Aiursoft.Observer.SDK.Models.EventViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Observer.Controllers;

[ApiModelStateChecker]
[ApiExceptionHandler]
public class EventController : ControllerBase
{
    private readonly CanonQueue _cannon;
    private readonly ObserverDbContext _dbContext;
    private readonly AiursoftAppTokenValidator _tokenManager;

    public EventController(
        AiursoftAppTokenValidator tokenManager,
        ObserverDbContext dbContext,
        CanonQueue cannon)
    {
        _tokenManager = tokenManager;
        _dbContext = dbContext;
        _cannon = cannon;
    }

    [HttpPost]
    public async Task<IActionResult> Log(LogAddressModel model)
    {
        var appid = await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        _cannon.QueueWithDependency<ObserverDbContext>(async dbContext =>
        {
            var newEvent = new ErrorLog
            {
                AppId = appid,
                Message = model.Message,
                StackTrace = model.StackTrace,
                EventLevel = model.EventLevel,
                Path = model.Path
            };
            dbContext.ErrorLogs.Add(newEvent);
            await dbContext.SaveChangesAsync();
        });

        return this.Protocol(Code.Success, "Successfully logged your event.");
    }

    [Produces(typeof(ViewLogViewModel))]
    public async Task<IActionResult> View(ViewAddressModel model)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        var logs = (await _dbContext
                .ErrorLogs
                .Where(t => t.AppId == appid)
                .ToListAsync())
            .GroupBy(t => t.Message)
            .Select(t => new LogCollection
            {
                Message = t.Key,
                First = t.OrderByDescending(p => p.LogTime).FirstOrDefault(),
                Count = t.Count()
            })
            .ToList();
        var viewModel = new ViewLogViewModel
        {
            AppId = appid,
            Logs = logs,
            Code = Code.Success,
            Message = "Successfully get your logs!"
        };
        return this.Protocol(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteApp(DeleteAppAddressModel model)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(model.AccessToken);
        if (appid != model.AppId)
        {
            return this.Protocol(Code.Unauthorized,
                "The app you try to delete is not the access token you granted!");
        }

        _dbContext.ErrorLogs.Delete(t => t.AppId == appid);
        await _dbContext.SaveChangesAsync();
        
        // TODO: Use NoActioNeeded for non exists apps.
        return this.Protocol(Code.Success, "App deleted.");
    }
}
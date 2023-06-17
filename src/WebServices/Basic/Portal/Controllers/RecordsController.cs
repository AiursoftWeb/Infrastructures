using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Portal.Data;
using Aiursoft.Portal.Models;
using Aiursoft.Portal.Models.RecordsViewModels;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Identity.Attributes;
using Aiursoft.Warpgate.SDK.Services.ToWarpgateServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Portal.Controllers;

[AiurForceAuth]
[LimitPerMin]
[Route("Dashboard")]
public class RecordsController : Controller
{
    private readonly DirectoryAppTokenService _directoryAppTokenService;
    private readonly RecordsService _recordsService;
    private readonly PortalDbContext _dbContext;

    public RecordsController(
        PortalDbContext dbContext,
        DirectoryAppTokenService directoryAppTokenService,
        RecordsService recordsService)
    {
        _dbContext = dbContext;
        _directoryAppTokenService = directoryAppTokenService;
        _recordsService = recordsService;
    }


    [Route("Apps/{appId}/CreateRecord")]
    public async Task<IActionResult> CreateRecord([FromRoute] string appId) // app id
    {
        var user = await GetCurrentUserAsync();
        var app = await _dbContext.Apps.FindAsync(appId);
        if (app == null)
        {
            return NotFound();
        }

        if (app.CreatorId != user.Id)
        {
            return Unauthorized();
        }

        var model = new CreateRecordViewModel(user)
        {
            AppId = appId,
            AppName = app.AppName
        };
        return View(model);
    }

    [HttpPost]
    [Route("Apps/{AppId}/CreateRecord")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRecord(CreateRecordViewModel model)
    {
        var user = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            model.Recover(user);
            return View(model);
        }

        var app = await _dbContext.Apps.FindAsync(model.AppId);
        if (app == null)
        {
            return NotFound();
        }

        if (app.CreatorId != user.Id)
        {
            return Unauthorized();
        }

        try
        {
            var token = await _directoryAppTokenService.GetAccessTokenWithAppInfoAsync(app.AppId, app.AppSecret);
            await _recordsService.CreateNewRecordAsync(token, model.RecordName, model.URL,
                model.Tags?.Split(',') ?? Array.Empty<string>(), model.Type, model.Enabled);
            return RedirectToAction(nameof(AppsController.ViewApp), "Apps",
                new { id = app.AppId, JustHaveUpdated = true });
        }
        catch (AiurUnexpectedResponse e)
        {
            ModelState.AddModelError(string.Empty, e.Response.Message);
            model.Recover(user);
            return View(model);
        }
    }

    [Route("Apps/{appId}/Records/{recordName}/Edit")]
    public async Task<IActionResult> Edit(
        [FromRoute] string appId,
        [FromRoute] string recordName)
    {
        var user = await GetCurrentUserAsync();
        var app = await _dbContext.Apps.FindAsync(appId);
        if (app == null)
        {
            return NotFound();
        }

        if (app.CreatorId != user.Id)
        {
            return Unauthorized();
        }

        var accessToken = _directoryAppTokenService.GetAccessTokenWithAppInfoAsync(app.AppId, app.AppSecret);
        var allRecords = await _recordsService.ViewMyRecordsAsync(await accessToken);
        var recordDetail = allRecords.Records.FirstOrDefault(t => t.RecordUniqueName == recordName);
        if (recordDetail == null)
        {
            return NotFound();
        }

        var model = new EditViewModel(user)
        {
            AppId = appId,
            OldRecordName = recordName,
            NewRecordName = recordName,
            AppName = app.AppName,
            Type = recordDetail.Type,
            URL = recordDetail.TargetUrl,
            Enabled = recordDetail.Enabled,
            Tags = recordDetail.Tags
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Apps/{appId}/Records/{oldRecordName}/Edit")]
    public async Task<IActionResult> Edit(EditViewModel model)
    {
        var user = await GetCurrentUserAsync();
        var app = await _dbContext.Apps.FindAsync(model.AppId);
        if (app == null)
        {
            return NotFound();
        }

        if (app.CreatorId != user.Id)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            model.Recover(user, app.AppName);
            return View(model);
        }

        try
        {
            var token = await _directoryAppTokenService.GetAccessTokenWithAppInfoAsync(app.AppId, app.AppSecret);
            await _recordsService.UpdateRecordInfoAsync(token, model.OldRecordName, model.NewRecordName, model.Type,
                model.URL, model.Tags?.Split(',') ?? Array.Empty<string>(), model.Enabled);
            return RedirectToAction(nameof(AppsController.ViewApp), "Apps",
                new { id = app.AppId, JustHaveUpdated = true });
        }
        catch (AiurUnexpectedResponse e)
        {
            ModelState.AddModelError(string.Empty, e.Response.Message);
            model.Recover(user, app.AppName);
            model.NewRecordName = model.OldRecordName;
            return View(model);
        }
    }

    [Route("Apps/{appId}/Records/{recordName}/Delete")]
    public async Task<IActionResult> Delete(
        [FromRoute] string appId,
        [FromRoute] string recordName)
    {
        var user = await GetCurrentUserAsync();
        var app = await _dbContext.Apps.FindAsync(appId);
        if (app == null)
        {
            return NotFound();
        }

        if (app.CreatorId != user.Id)
        {
            return Unauthorized();
        }

        var model = new DeleteViewModel(user)
        {
            AppId = appId,
            RecordName = recordName,
            AppName = app.AppName
        };
        return View(model);
    }

    [HttpPost]
    [Route("Apps/{appId}/Records/{recordName}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(DeleteViewModel model)
    {
        var user = await GetCurrentUserAsync();
        var app = await _dbContext.Apps.FindAsync(model.AppId);
        if (app == null)
        {
            return NotFound();
        }

        if (app.CreatorId != user.Id)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            model.Recover(user, app.AppName);
            return View(model);
        }

        try
        {
            var token = await _directoryAppTokenService.GetAccessTokenWithAppInfoAsync(app.AppId, app.AppSecret);
            await _recordsService.DeleteRecordAsync(token, model.RecordName);
            return RedirectToAction(nameof(AppsController.ViewApp), "Apps",
                new { id = app.AppId, JustHaveUpdated = true });
        }
        catch (AiurUnexpectedResponse e)
        {
            ModelState.AddModelError(string.Empty, e.Response.Message);
            model.Recover(user, app.AppName);
            return View(model);
        }
    }

    private async Task<PortalUser> GetCurrentUserAsync()
    {
        return await _dbContext.Users.Include(t => t.MyApps)
            .SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
    }
}
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models;
using Aiursoft.Developer.Models.RecordsViewModels;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Identity.Attributes;
using Aiursoft.Wrapgate.SDK.Services.ToWrapgateServer;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [AiurForceAuth]
    [LimitPerMin]
    [Route("Dashboard")]
    public class RecordsController : Controller
    {
        public DeveloperDbContext _dbContext;
        private readonly AppsContainer _appsContainer;
        private readonly RecordsService _recordsService;
        private readonly AiurCache _cache;

        public RecordsController(
            DeveloperDbContext dbContext,
            AppsContainer appsContainer,
            RecordsService recordsService,
            AiurCache cache)
        {
            _dbContext = dbContext;
            _appsContainer = appsContainer;
            _recordsService = recordsService;
            _cache = cache;
        }


        [Route("Apps/{appId}/CreateRecord")]
        public async Task<IActionResult> CreateRecord([FromRoute] string appId)// app id
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
                model.ModelStateValid = false;
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
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                await _recordsService.CreateNewRecordAsync(token, model.RecordName, model.URL, model.Type, model.Enabled);
                return RedirectToAction(nameof(AppsController.ViewApp), "Apps", new { id = app.AppId, JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user);
                return View(model);
            }
        }

        [Route("Apps/{appId}/Records/{recordName}/Edit")]
        public async Task<IActionResult> Edit(string appId, string recordName)
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
            var accesstoken = _appsContainer.AccessToken(app.AppId, app.AppSecret);
            var allRecords = await _recordsService.ViewMyRecordsAsync(await accesstoken);
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
                Enabled = recordDetail.Enabled
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
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                await _recordsService.UpdateRecordInfoAsync(token, model.OldRecordName, model.NewRecordName, model.Type, model.URL, model.Enabled);
                _cache.Clear($"Record-public-status-{model.OldRecordName}");
                _cache.Clear($"Record-public-status-{model.NewRecordName}");
                return RedirectToAction(nameof(AppsController.ViewApp), "Apps", new { id = app.AppId, JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                model.NewRecordName = model.OldRecordName;
                return View(model);
            }
        }

        [Route("Apps/{appId}/Records/{recordName}/Delete")]
        public async Task<IActionResult> Delete(string appId, string recordName)
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
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.AccessToken(app.AppId, app.AppSecret);
                await _recordsService.DeleteRecordAsync(token, model.RecordName);
                return RedirectToAction(nameof(AppsController.ViewApp), "Apps", new { id = app.AppId, JustHaveUpdated = true });
            }
            catch (AiurUnexceptedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.ModelStateValid = false;
                model.Recover(user, app.AppName);
                return View(model);
            }
        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}

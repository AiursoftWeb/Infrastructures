using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Identity.Attributes;
using Aiursoft.Wrap.Models;
using Aiursoft.Wrap.Models.DashboardViewModels;
using Aiursoft.Wrapgate.SDK.Models;
using Aiursoft.Wrapgate.SDK.Services.ToWrapgateServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wrap.Controllers
{
    [LimitPerMin]
    [AiurForceAuth]
    [Route("Dashboard")]
    public class DashboardController : Controller
    {
        private readonly AppsContainer _appsContainer;
        private readonly RecordsService _recordsService;
        private readonly UserManager<WrapUser> _userManager;

        public DashboardController(
            AppsContainer appsContainer,
            RecordsService recordsService,
            UserManager<WrapUser> userManager)
        {
            _appsContainer = appsContainer;
            _recordsService = recordsService;
            _userManager = userManager;
        }

        [Route(nameof(Index))]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

        [HttpPost]
        [Route(nameof(Create))]
        public async Task<IActionResult> Create(IndexViewModel model)
        {
            var token = await _appsContainer.AccessToken();
            var user = await GetCurrentUserAsync();
            try
            {
                await _recordsService.CreateNewRecordAsync(token, model.NewRecordName, model.Url, new[] { user.Id }, RecordType.Redirect, enabled: true);
            }
            catch (AiurUnexpectedResponse e) when (e.Code == ErrorType.Conflict)
            {
                ModelState.AddModelError(nameof(model.NewRecordName), $"Sorry but the key:'{model.NewRecordName}' already exists. Try another one.");
                model.Recover(user);
                return View(nameof(Index), model);
            }
#warning Index is ugly!
            model.Recover(user);
            return View("Created", model);
        }

        [Route(nameof(Records))]
        public async Task<IActionResult> Records()
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var records = await _recordsService.ViewMyRecordsAsync(token, user.Id);
            var model = new RecordsViewModel(user, records.Records);
            return View(model);
        }

        [Route("Records/{recordName}/Edit")]
        public async Task<IActionResult> Edit([FromRoute] string recordName)
        {
            var user = await GetCurrentUserAsync();
            var accessToken = _appsContainer.AccessToken();
            var allRecords = await _recordsService.ViewMyRecordsAsync(await accessToken);
            var recordDetail = allRecords.Records.FirstOrDefault(t => t.RecordUniqueName == recordName);
            if (recordDetail == null)
            {
                return NotFound();
            }
            var model = new EditViewModel(user)
            {
                RecordName = recordName,
                NewRecordName = recordName,
                Type = recordDetail.Type,
                URL = recordDetail.TargetUrl,
                Enabled = recordDetail.Enabled
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Records/{recordName}/Edit")]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(user);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.AccessToken();
                await _recordsService.UpdateRecordInfoAsync(token, model.RecordName, model.NewRecordName, model.Type, model.URL, new[] { user.Id }, model.Enabled);
                return RedirectToAction(nameof(DashboardController.Records), "Dashboard");
            }
            catch (AiurUnexpectedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.Recover(user);
                model.NewRecordName = model.RecordName;
                return View(model);
            }
        }

        [Route("Records/{recordName}/Delete")]
        public async Task<IActionResult> Delete([FromRoute] string recordName)
        {
            var user = await GetCurrentUserAsync();
            var model = new DeleteViewModel(user)
            {
                RecordName = recordName
            };
            return View(model);
        }

        [HttpPost]
        [Route("Records/{recordName}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(user);
                return View(model);
            }
            try
            {
                var token = await _appsContainer.AccessToken();
                await _recordsService.DeleteRecordAsync(token, model.RecordName);
                return RedirectToAction(nameof(DashboardController.Records), "Dashboard");
            }
            catch (AiurUnexpectedResponse e)
            {
                ModelState.AddModelError(string.Empty, e.Response.Message);
                model.Recover(user);
                return View(model);
            }
        }

        private async Task<WrapUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}

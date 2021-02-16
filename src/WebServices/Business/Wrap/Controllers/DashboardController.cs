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
using System.Threading.Tasks;

namespace Aiursoft.Wrap.Controllers
{
    [LimitPerMin]
    [AiurForceAuth]
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

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

        [HttpPost]
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
            #warning  No Created view!
            return View("Created", model);
        }

        public async Task<IActionResult> Records()
        {
            var user = await GetCurrentUserAsync();
            var token = await _appsContainer.AccessToken();
            var records = await _recordsService.ViewMyRecordsAsync(token, user.Id);
            var model = new RecordsViewModel(user, records.Records);
            return View(model);
        }

        private async Task<WrapUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}

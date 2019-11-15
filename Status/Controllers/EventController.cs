using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Status;
using Aiursoft.Pylon.Models.Status.EventAddressModels;
using Aiursoft.Pylon.Models.Status.EventViewModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Status.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Status.Controllers
{
    [LimitPerMin]
    [APIModelStateChecker]
    [APIExpHandler]
    public class EventController : Controller
    {
        private readonly ACTokenManager _tokenManager;
        private readonly StatusDbContext _dbContext;

        public EventController(
            ACTokenManager tokenManager,
            StatusDbContext dbContext)
        {
            _tokenManager = tokenManager;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Log(LogAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.StatusApps.SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new StatusApp
                {
                    AppId = appid
                };
                _dbContext.StatusApps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }
            var newEvent = new ErrorLog
            {
                AppId = appid,
                Message = model.Message,
                StackTrace = model.StackTrace,
                EventLevel = model.EventLevel
            };
            _dbContext.ErrorLogs.Add(newEvent);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"Successfully logged your event.");
        }

        [APIProduces(typeof(ViewLogViewModel))]
        public async Task<IActionResult> View(ViewAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.StatusApps.SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new StatusApp
                {
                    AppId = appid
                };
                _dbContext.StatusApps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }

            var logs = _dbContext
                .ErrorLogs
                .Where(t => t.AppId == appid)
                .GroupBy(t => t.Message)
                .AsEnumerable()
                .Select(t => new LogCollection
                {
                    Message = t.Key,
                    First = t.FirstOrDefault(),
                    Count = t.Count()
                })
                .ToList();
            var viewModel = new ViewLogViewModel
            {
                AppId = appLocal.AppId,
                Logs = logs,
                Code = ErrorType.Success,
                Message = "Successfully get your logs!"
            };
            return Json(viewModel);
        }
    }
}

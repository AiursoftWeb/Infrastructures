using Aiursoft.Archon.SDK.Services;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Pylon;
using Aiursoft.Status.Data;
using Aiursoft.Status.SDK.Models;
using Aiursoft.Status.SDK.Models.EventAddressModels;
using Aiursoft.Status.SDK.Models.EventViewModels;
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
                EventLevel = model.EventLevel,
                Path = model.Path
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
                AppId = appLocal.AppId,
                Logs = logs,
                Code = ErrorType.Success,
                Message = "Successfully get your logs!"
            };
            return Json(viewModel);
        }
    }
}

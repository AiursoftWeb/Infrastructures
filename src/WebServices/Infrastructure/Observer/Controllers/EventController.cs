using Aiursoft.Archon.SDK.Services;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.Data;
using Aiursoft.Observer.SDK.Models;
using Aiursoft.Observer.SDK.Models.EventAddressModels;
using Aiursoft.Observer.SDK.Models.EventViewModels;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Observer.Controllers
{
    [LimitPerMin]
    [APIModelStateChecker]
    [APIExpHandler]
    public class EventController : ControllerBase
    {
        private readonly ACTokenValidator _tokenManager;
        private readonly ObserverDbContext _dbContext;
        private readonly static SemaphoreSlim readLock = new SemaphoreSlim(1, 1);

        public EventController(
            ACTokenValidator tokenManager,
            ObserverDbContext dbContext)
        {
            _tokenManager = tokenManager;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Log(LogAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            await readLock.WaitAsync();
            try
            {
                var appLocal = await _dbContext.ObserverApps.SingleOrDefaultAsync(t => t.AppId == appid);
                if (appLocal == null)
                {
                    appLocal = new ObserverApp
                    {
                        AppId = appid
                    };
                    await _dbContext.ObserverApps.AddAsync(appLocal);
                    await _dbContext.SaveChangesAsync();
                }
            }
            finally
            {
                readLock.Release();
            }
            var newEvent = new ErrorLog
            {
                AppId = appid,
                Message = model.Message,
                StackTrace = model.StackTrace,
                EventLevel = model.EventLevel,
                Path = model.Path
            };
            await _dbContext.ErrorLogs.AddAsync(newEvent);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully logged your event.");
        }

        [APIProduces(typeof(ViewLogViewModel))]
        public async Task<IActionResult> View(ViewAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.ObserverApps.SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new ObserverApp
                {
                    AppId = appid
                };
                await _dbContext.ObserverApps.AddAsync(appLocal);
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
            return Ok(viewModel);
        }
    }
}

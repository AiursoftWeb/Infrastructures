using Aiursoft.Archon.SDK.Services;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.Data;
using Aiursoft.Observer.SDK.Models;
using Aiursoft.Observer.SDK.Models.EventAddressModels;
using Aiursoft.Observer.SDK.Models.EventViewModels;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly CannonQueue queue;

        public EventController(
            ACTokenValidator tokenManager,
            ObserverDbContext dbContext,
            CannonQueue queue)
        {
            _tokenManager = tokenManager;
            _dbContext = dbContext;
            this.queue = queue;
        }

        [HttpPost]
        public IActionResult Log(LogAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            queue.QueueWithDependency<ObserverDbContext>(async dbContext =>
            {
                var newEvent = new ErrorLog
                {
                    AppId = appid,
                    Message = model.Message,
                    StackTrace = model.StackTrace,
                    EventLevel = model.EventLevel,
                    Path = model.Path
                };
                await dbContext.ErrorLogs.AddAsync(newEvent);
                await dbContext.SaveChangesAsync();
            });

            return this.Protocol(ErrorType.Success, "Successfully logged your event.");
        }

        [APIProduces(typeof(ViewLogViewModel))]
        public async Task<IActionResult> View(ViewAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
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
                Code = ErrorType.Success,
                Message = "Successfully get your logs!"
            };
            return Ok(viewModel);
        }
    }
}

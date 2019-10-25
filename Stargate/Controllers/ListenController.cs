using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Stargate.ListenAddressModels;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Controllers
{
    [LimitPerMin]
    [APIExpHandler]
    [APIModelStateChecker]
    public class ListenController : Controller
    {
        private StargateDbContext _dbContext;
        private StargateMemory _memoryContext;
        private WebSocketPusher _pusher;
        private readonly ILogger<ListenController> _logger;

        public ListenController(
            StargateDbContext dbContext,
            StargateMemory memoryContext,
            WebSocketPusher pusher,
            ILogger<ListenController> logger)
        {
            _dbContext = dbContext;
            _memoryContext = memoryContext;
            _pusher = pusher;
            _logger = logger;
        }

        [AiurForceWebSocket]
        public async Task<IActionResult> Channel(ChannelAddressModel model)
        {
            int lastReadId = 0;
            var channel = await _dbContext.Channels.FindAsync(model.Id);
            if (channel == null)
            {
                return this.Protocol(ErrorType.NotFound, "Can not find channel with id: " + model.Id);
            }
            if (channel.ConnectKey != model.Key)
            {
                return Json(new AiurProtocol
                {
                    Code = ErrorType.Unauthorized,
                    Message = "Wrong connection key!"
                });
            }
            await _pusher.Accept(HttpContext);
            int sleepTime = 0;
            while (_pusher.Connected)
            {
                try
                {
                    var nextMessages = _memoryContext
                        .Messages
                        .Where(t => t.ChannelId == model.Id)
                        .Where(t => t.Id > lastReadId)
                        .ToList();
                    if (!nextMessages.Any())
                    {
                        if (sleepTime < 1000)
                            sleepTime += 5;
                        await Task.Delay(sleepTime);
                    }
                    else
                    {
                        var nextMessage = nextMessages.OrderBy(t => t.Id).FirstOrDefault();
                        if (nextMessage != null)
                        {
                            await _pusher.SendMessage(nextMessage.Content);
                            lastReadId = nextMessage.Id;
                            sleepTime = 0;
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
            return Json(new { });
        }
    }
}
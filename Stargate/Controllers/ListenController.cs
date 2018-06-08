using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon;
using Aiursoft.Stargate.Data;
using System.Net.WebSockets;
using Aiursoft.Stargate.Services;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Stargate.ListenAddressModels;

namespace Aiursoft.Stargate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class ListenController : Controller
    {
        private StargateDbContext _dbContext;
        private StargateMemory _memoryContext;
        private IPusher<WebSocket> _pusher;

        public ListenController(StargateDbContext dbContext,
            StargateMemory memoryContext,
            IPusher<WebSocket> pusher)
        {
            _dbContext = dbContext;
            _memoryContext = memoryContext;
            _pusher = pusher;
        }

        [AiurForceWebSocket]
        public async Task<IActionResult> Channel(ChannelAddressModel model)
        {
            var lastReadTime = DateTime.Now;
            var channel = await _dbContext.Channels.FindAsync(model.Id);
            if (channel.ConnectKey != model.Key)
            {
                return Json(new AiurProtocal
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
                        .Where(t => t.CreateTime > lastReadTime)
                        .ToList();
                    if (!nextMessages.Any())
                    {
                        if (sleepTime < 1000)
                            sleepTime += 5;
                        await Task.Delay(sleepTime);
                    }
                    else
                    {
                        var nextMessage = nextMessages.OrderBy(t => t.CreateTime).First();
                        await _pusher.SendMessage(nextMessage.Content);
                        lastReadTime = nextMessage.CreateTime;
                        sleepTime = 0;
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return null;
        }
    }
}
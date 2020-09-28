using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.SDK.Models;
using Aiursoft.Observer.SDK.Services.ToStatusServer;
using Aiursoft.Stargate.Attributes;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.Stargate.Services;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Services;
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
    public class ListenController : ControllerBase
    {
        private readonly Counter _counter;
        private readonly StargateDbContext _dbContext;
        private readonly StargateMemory _memoryContext;
        private readonly WebSocketPusher _pusher;
        private readonly ILogger<ListenController> _logger;
        private readonly AppsContainer _appsContainer;
        private readonly EventService _eventService;
        private readonly ConnectedCountService _connectedCountService;
        private readonly LastAccessService _lastAccessService;
        private readonly ChannelLiveJudge _channelLiveJudge;

        public ListenController(
            StargateDbContext dbContext,
            StargateMemory memoryContext,
            WebSocketPusher pusher,
            ILogger<ListenController> logger,
            Counter counter,
            AppsContainer appsContainer,
            EventService eventService,
            ConnectedCountService connectedCountService,
            LastAccessService lastAccessService,
            ChannelLiveJudge channelLiveJudge)
        {
            _dbContext = dbContext;
            _memoryContext = memoryContext;
            _pusher = pusher;
            _logger = logger;
            _counter = counter;
            _appsContainer = appsContainer;
            _eventService = eventService;
            _connectedCountService = connectedCountService;
            _lastAccessService = lastAccessService;
            _channelLiveJudge = channelLiveJudge;
        }

        [AiurForceWebSocket]
        public async Task<IActionResult> Channel(ChannelAddressModel model)
        {
            int lastReadId = _counter.GetCurrent;
            var channel = await _dbContext.Channels.FindAsync(model.Id);
            if (channel == null)
            {
                return this.Protocol(ErrorType.NotFound, "Can not find channel with id: " + model.Id);
            }
            if (channel.ConnectKey != model.Key)
            {
                return Ok(new AiurProtocol
                {
                    Code = ErrorType.Unauthorized,
                    Message = "Wrong connection key!"
                });
            }
            await _pusher.Accept(HttpContext);
            int sleepTime = 0;
            try
            {
                _connectedCountService.AddConnectedCount(channel.Id);
                await Task.Run(_pusher.PendingClose);
                _lastAccessService.RecordLastConnectTime(channel.Id);
                while (_pusher.Connected && _channelLiveJudge.IsAlive(channel.Id))
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
                _lastAccessService.RecordLastConnectTime(channel.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                var accessToken = _appsContainer.AccessToken();
                await _eventService.LogExceptionAsync(await accessToken, e, Request.Path);
            }
            finally
            {
                _connectedCountService.ReduceConnectedCount(channel.Id);
            }
            return Ok(new { });
        }
    }
}
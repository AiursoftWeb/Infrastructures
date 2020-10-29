using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.Stargate.SDK.Services.ToStargateServer;
using Aiursoft.Stargate.Services;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Controllers
{
    [LimitPerMin]
    [APIExpHandler]
    [APIModelStateChecker]
    public class HomeController : Controller
    {
        private readonly AppsContainer _appsContainer;
        private readonly ChannelService _channelService;
        private readonly Counter _counter;
        private readonly StargateMemory _memory;
        private readonly CannonService _cannonService;

        public HomeController(
            AppsContainer appsContainer,
            ChannelService channelService,
            Counter counter,
            StargateMemory memory,
            CannonService cannonService)
        {
            _appsContainer = appsContainer;
            _channelService = channelService;
            _counter = counter;
            _memory = memory;
            _cannonService = cannonService;
        }

        public IActionResult Index()
        {
            var (channels, messages) = _memory.GetMonitoringReport();
            return Json(new
            {
                CurrentId = _counter.GetCurrent,
                TotalMemoryMessages = messages,
                Channels = channels,
                Code = ErrorType.Success,
                Message = "Welcome to Aiursoft Stargate server!"
            });
        }

        [LimitPerMin(2)]
        public async Task<IActionResult> IntegratedTest()
        {
            var token = await _appsContainer.AccessToken();
            var result = await _channelService.CreateChannelAsync(token, "Test Channel");
            _cannonService.FireAsync<DebugMessageSender>(d => 
            {
                return d.SendDebuggingMessages(token, result.ChannelId);
            });
            var model = new ChannelAddressModel
            {
                Id = result.ChannelId,
                Key = result.ConnectKey
            };
            return View("Test", model);
        }

        public IActionResult ListenTo(ChannelAddressModel model)
        {
            return View("Test", model);
        }

        public IActionResult Error()
        {
            return this.Protocol(ErrorType.UnknownError, "Stargate server crashed! Please tell us!");
        }
    }
}

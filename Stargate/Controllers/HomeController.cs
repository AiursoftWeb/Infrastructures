using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Stargate.ListenAddressModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToStargateServer;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class HomeController : Controller
    {
        private StargateDbContext _dbContext;
        private DebugMessageSender _debugger;
        private readonly AppsContainer _appsContainer;
        private readonly ChannelService _channelService;
        private readonly Counter _counter;
        private readonly StargateMemory _memory;

        public HomeController(
            StargateDbContext dbContext,
            DebugMessageSender debugger,
            AppsContainer appsContainer,
            ChannelService channelService,
            Counter counter,
            StargateMemory memory)
        {
            _dbContext = dbContext;
            _debugger = debugger;
            _appsContainer = appsContainer;
            _channelService = channelService;
            _counter = counter;
            _memory = memory;
        }

        public IActionResult Index()
        {
            return Json(new
            {
                CurrentId = _counter.GetCurrent,
                TotalMemoryMessages = _memory.Messages.Count,
                Threads = Process.GetCurrentProcess().Threads.Count,
                Code = ErrorType.Success,
                Message = "Welcome to Aiursoft Message queue server!"
            });
        }

        public async Task<IActionResult> IntegratedTest()
        {
            var token = await _appsContainer.AccessToken();
            var result = await _channelService.CreateChannelAsync(token, "Test Channel");
            await Task.Factory.StartNew(async () =>
            {
                await _debugger.SendDebuggingMessages(token, result.ChannelId);
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

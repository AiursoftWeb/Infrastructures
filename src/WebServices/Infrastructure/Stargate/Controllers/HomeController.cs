using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Stargate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class HomeController : Controller
    {
        private readonly Counter _counter;
        private readonly StargateMemory _memory;

        public HomeController(
            StargateMemory memory,
            Counter counter)
        {
            _memory = memory;
            _counter = counter;
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

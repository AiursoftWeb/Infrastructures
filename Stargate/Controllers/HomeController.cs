using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.ToStargateServer;
using Aiursoft.Pylon;
using Aiursoft.Stargate.Services;
using Aiursoft.Pylon.Models.Stargate.ListenAddressModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Stargate.Data;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Models.Stargate;
using Aiursoft.Pylon.Services;

namespace Aiursoft.Stargate.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class HomeController : Controller
    {
        private StargateDbContext _dbContext;
        private Debugger _debugger;
        public HomeController(
            StargateDbContext dbContext,
            Debugger debugger)
        {
            _dbContext = dbContext;
            _debugger = debugger;
        }
        public IActionResult Index()
        {
            return Json(new AiurProtocal
            {
                code = ErrorType.Success,
                message = "Welcome to Aiursoft Message queue server!"
            });
        }

        public async Task<IActionResult> IntegratedTest()
        {
            var token = AppsContainer.AccessToken();
            var result = await ChannelService.CreateChannelAsync(await token(), "Test Channel");
            await Task.Factory.StartNew(async () =>
            {
                await _debugger.SendDebuggingMessages(await token(), result.ChannelId);
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
            return this.Protocal(ErrorType.UnknownError, "Stargate server crashed! Please tell us!");
        }
    }
}

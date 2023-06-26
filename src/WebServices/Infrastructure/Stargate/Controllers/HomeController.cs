using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.SDK.Models.HomeViewModels;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Aiursoft.CSTools.Services;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Stargate.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
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
        return this.Protocol(new IndexViewModel
        {
            CurrentId = _counter.GetCurrent,
            TotalMemoryMessages = messages,
            Channels = channels,
            Code = Code.ResultShown,
            Message = "Welcome to Aiursoft Stargate server!"
        });
    }

    public IActionResult ListenTo(ChannelAddressModel model)
    {
        return View("Test", model);
    }

    public IActionResult Error()
    {
        return this.Protocol(Code.UnknownError, "Stargate server crashed! Please tell us!");
    }
}
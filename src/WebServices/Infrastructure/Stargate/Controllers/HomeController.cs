using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Stargate.SDK.Models.ListenAddressModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Stargate.Controllers;

[ApiExceptionHandler]
[ApiModelStateChecker]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return this.Protocol(new AiurResponse
        {
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
using Aiursoft.AiurProtocol.Models;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.AiurProtocol.Server.Attributes;

namespace Aiursoft.Observer.Controllers;

[ApiModelStateChecker]
[ApiExceptionHandler]
public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return this.Protocol(Code.NoActionTaken, "Welcome to Aiursoft Observer!");
    }
}
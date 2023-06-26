using Microsoft.AspNetCore.Mvc;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;

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
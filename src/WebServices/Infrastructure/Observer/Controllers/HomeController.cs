using Aiursoft.AiurProtocol.Models;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.AiurProtocol;

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
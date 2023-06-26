using Aiursoft.AiurProtocol;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aiursoft.Stargate.Attributes;

public class AiurForceWebSocket : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        if (context.HttpContext.WebSockets.IsWebSocketRequest)
        {
            return;
        }

        var arg = new AiurResponse
        {
            Code = Code.InvalidInput,
            Message = "Wrong protocol! Please connect with Websocket instead of HTTP!"
        };
        context.Result = new JsonResult(arg);
    }
}
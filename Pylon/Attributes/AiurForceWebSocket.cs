using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Attributes
{
    public class AiurForceWebSocket : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!context.HttpContext.WebSockets.IsWebSocketRequest)
            {
                var arg = new AiurProtocal
                {
                    code = ErrorType.InvalidInput,
                    message = "Wrong protocal!"
                };
                context.Result = new JsonResult(arg);
            }
        }
    }
}

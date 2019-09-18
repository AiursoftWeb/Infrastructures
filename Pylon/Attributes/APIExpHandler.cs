using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aiursoft.Pylon.Attributes
{
    /// <summary>
    /// Adding this will handle `AiurAPIModelException` and return the result as JSON directly.
    /// Adding this will handle `AiurUnexceptedResponse` and return the result as JSON directly.
    /// </summary>
    public class APIExpHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            switch (context.Exception)
            {
                case AiurUnexceptedResponse exp:
                    context.ExceptionHandled = true;
                    context.Result = new JsonResult(new AiurProtocol { Code = exp.Code, Message = exp.Message });
                    break;
                case AiurAPIModelException exp:
                    context.ExceptionHandled = true;
                    context.Result = new JsonResult(new AiurProtocol { Code = exp.Code, Message = exp.Message });
                    break;
                    // For other exception types will throw it directly.
            }
        }
    }
}

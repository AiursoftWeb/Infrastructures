using Aiursoft.Handler.Abstract.Exceptions;
using Aiursoft.Handler.Abstract.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aiursoft.Handler.Attributes
{
    /// <summary>
    /// Adding this will handle `AiurAPIModelException` when the code is not found and return 404.
    /// Adding this will handle `AiurUnexceptedResponse` when the code is not found and return 404.
    /// </summary>
    public class ProtocolNotfoundHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            switch (context.Exception)
            {
                case AiurUnexceptedResponse exp:
                    if (exp.Code == ErrorType.NotFound)
                    {
                        context.ExceptionHandled = true;
                        context.Result = new NotFoundResult();
                    }
                    break;
                case AiurAPIModelException exp:
                    if (exp.Code == ErrorType.NotFound)
                    {
                        context.ExceptionHandled = true;
                        context.Result = new NotFoundResult();
                    }
                    break;
                    // For other exception types will throw it directly.
            }
        }
    }
}

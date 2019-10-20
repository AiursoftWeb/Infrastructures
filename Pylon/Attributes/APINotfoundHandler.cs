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
    public class APINotfoundHandler : ExceptionFilterAttribute
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

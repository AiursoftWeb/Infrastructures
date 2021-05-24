using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Aiursoft.Handler.Attributes
{
    /// <summary>
    /// This attribute handles all AiurUnexpectedResponse when the response is not found. And will provide a user friendly not found result.
    /// </summary>
    public class UserFriendlyNotFoundHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            if (context.Exception is AiurUnexpectedResponse unexpectedResponse && unexpectedResponse.Code == ErrorType.NotFound)
            {
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Result = new NotFoundResult();
            }
        }
    }
}

using Aiursoft.Handler.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aiursoft.Handler.Attributes
{
    /// <summary>
    /// This attribute will not throw any exception but will reject any invalid request directly with AiurCollection with error messages.
    /// Strongly suggest adding this attribute to all API controllers.
    /// </summary>
    public class APIModelStateChecker : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var (result, code) = ResultGenerator.GetInvalidModelStateErrorResponse(context.ModelState);
                context.Result = result;
                context.HttpContext.Response.StatusCode = (int)code;
            }
            base.OnActionExecuting(context);
        }
    }
}

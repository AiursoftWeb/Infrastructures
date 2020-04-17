using Microsoft.AspNetCore.Mvc.Filters;

namespace Aiursoft.Pylon.Attributes
{
    // ReSharper disable once UnusedMember.Global
    public class AiurNoCache : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!context.HttpContext.Response.Headers.ContainsKey("Cache-Control"))
            {
                context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache");
            }
            if (!context.HttpContext.Response.Headers.ContainsKey("Expires"))
            {
                context.HttpContext.Response.Headers.Add("Expires", "-1");
            }
        }
    }
}

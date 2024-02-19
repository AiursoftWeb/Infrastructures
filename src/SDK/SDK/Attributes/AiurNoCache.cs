using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Aiursoft.SDK.Attributes;

// ReSharper disable once UnusedMember.Global
public class AiurNoCache : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        if (!context.HttpContext.Response.Headers.ContainsKey("Cache-Control"))
        {
            context.HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        }

        if (!context.HttpContext.Response.Headers.ContainsKey("Expires"))
        {
            context.HttpContext.Response.Headers.Append("Expires", "-1");
        }
    }
}
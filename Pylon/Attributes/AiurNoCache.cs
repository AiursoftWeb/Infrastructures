using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Attributes
{
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

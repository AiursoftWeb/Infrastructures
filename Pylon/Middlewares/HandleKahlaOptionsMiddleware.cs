using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Middlewares
{
    public class HandleKahlaOptionsMiddleware
    {
        private RequestDelegate _next;

        public HandleKahlaOptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Cache-Control", "no-cache");
            context.Response.Headers.Add("Expires", "-1");
            if (context.Request.Path.Value.ToLower().Contains("debug"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:8001");
            }
            else
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", Values.KahlaAppAddress);
            }
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 204;
                return;
            }
            await _next.Invoke(context);
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Middlewares
{
    public class HandleRobotsMiddleware
    {
        private readonly RequestDelegate _next;

        public HandleRobotsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (string.Equals(context.Request.Path, "/robots.txt", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}

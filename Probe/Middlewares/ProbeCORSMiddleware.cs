using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Middlewares
{
    public class ProbeCORSMiddleware
    {
        private readonly RequestDelegate _next;

        public ProbeCORSMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 204;
                return;
            }
            await _next.Invoke(context);
        }
    }
}

using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Middlewares
{
    public class UserFriendlyNotFoundMiddeware
    {
        private readonly RequestDelegate _next;
        private readonly ServiceLocation _serviceLocation;

        public UserFriendlyNotFoundMiddeware(
            RequestDelegate next,
            ServiceLocation serviceLocation)
        {
            _next = next;
            _serviceLocation = serviceLocation;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound && !context.Response.HasStarted)
            {
                var text = $"<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>404 Not found - Aiursoft</title><link rel=\"icon\" type=\"image/x-icon\" href=\"{_serviceLocation.UI}/favicon.ico\"><link href=\"{_serviceLocation.UI}/dist/AiurCore.min.css\" rel=\"stylesheet\"><link href=\"{_serviceLocation.UI}/dist/AiurDashboard.min.css\" rel=\"stylesheet\"></head><body class=\"bg-dark lines\"><div class=\"container\"><div class=\"card card-register mx-auto mt-5\"><div class=\"card-header\">404 Not found</div><div class=\"card-body\"><h1 class=\"text-danger\">404 - Not found</h1><h3>The target url you view is not found.</h3><p>The target URL you viewed is not found..</p></div><a class=\"card-footer clearfix small z-1\" href=\"{_serviceLocation.WWW}\"><span class=\"float-left\">Back to Home</span><span class=\"float-right\"><i class=\"fa fa-angle-right\"></i></span></a></div></div><script src=\"{_serviceLocation.UI}/dist/AiurCore.min.js\"></script><script src=\"{_serviceLocation.UI}/dist/AiurDashboard.min.js\"></script></body></html>";
                await context.Response.WriteAsync(text, Encoding.UTF8);
            }
        }
    }
}

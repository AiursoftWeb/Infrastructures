using System.Net;
using System.Text;
using System.Threading.Tasks;
using Aiursoft.SDK.Services;
using Microsoft.AspNetCore.Http;

namespace Aiursoft.SDK.Middlewares;

public class UserFriendlyBadRequestMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ServiceLocation _serviceLocation;

    public UserFriendlyBadRequestMiddleware(
        RequestDelegate next,
        ServiceLocation serviceLocation)
    {
        _next = next;
        _serviceLocation = serviceLocation;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next.Invoke(context);
        if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest && !context.Response.HasStarted)
        {
            var text =
                $"<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>400 Bad request - Aiursoft</title><link rel=\"icon\" type=\"image/x-icon\" href=\"{_serviceLocation.UI}/favicon.ico\"><link href=\"{_serviceLocation.UI}/dist/AiurDashboard.min.css\" rel=\"stylesheet\"></head><body class=\"bg-light lines\"><div class=\"container\"><div class=\"card card-register mx-auto mt-5\"><div class=\"card-header\">400 Bad request</div><div class=\"card-body\"><h1 class=\"text-danger\">400 - Bad request</h1><h3>Some error occurred with the request and cannot be processed.</h3><p>Some error occurred with the request and cannot be processed.</p></div><a class=\"card-footer clearfix small z-1\" href=\"{_serviceLocation.WWW}\"><span class=\"float-left\">Back to Home</span><span class=\"float-right\"><i class=\"fa fa-angle-right\"></i></span></a></div></div><script src=\"{_serviceLocation.UI}/dist/AiurDashboard.min.js\"></script></body></html>";
            await context.Response.WriteAsync(text, Encoding.UTF8);
        }
    }
}
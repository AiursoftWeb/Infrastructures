using Aiursoft.Pylon.Models.Status;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToStatusServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Middlewares
{
    public class UserFriendlyServerExceptionMiddeware
    {
        private readonly RequestDelegate _next;
        private readonly ServiceLocation _serviceLocation;
        private readonly ILogger<UserFriendlyServerExceptionMiddeware> _logger;
        private readonly EventService _eventService;
        private readonly AppsContainer _appsContainer;

        public UserFriendlyServerExceptionMiddeware(
            RequestDelegate next,
            ServiceLocation serviceLocation,
            ILogger<UserFriendlyServerExceptionMiddeware> logger,
            EventService eventService,
            AppsContainer appsContainer)
        {
            _next = next;
            _serviceLocation = serviceLocation;
            _logger = logger;
            _eventService = eventService;
            _appsContainer = appsContainer;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var text = $"<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>500 Server Exception - Aiursoft</title><link rel=\"icon\" type=\"image/x-icon\" href=\"{_serviceLocation.UI}/favicon.ico\"><link href=\"{_serviceLocation.UI}/dist/AiurCore.min.css\" rel=\"stylesheet\"><link href=\"{_serviceLocation.UI}/dist/AiurDashboard.min.css\" rel=\"stylesheet\"></head><body class=\"bg-light lines\"><div class=\"container\"><div class=\"card card-register mx-auto mt-5\"><div class=\"card-header\">500 Server Exception</div><div class=\"card-body\"><h1 class=\"text-danger\">500 - Server Exception</h1><h3>This operation caused an error on our server.</h3><p>Sorry about it. Please try again and tell us about the problem.</p><p>Or you can <a href=\"{_serviceLocation.Status}\" target=\"_blank\">view service health status</a> or submit an issue to our team.</p></div><a class=\"card-footer clearfix small z-1\" href=\"{_serviceLocation.WWW}\"><span class=\"float-left\">Back to Home</span><span class=\"float-right\"><i class=\"fa fa-angle-right\"></i></span></a></div></div><script src=\"{_serviceLocation.UI}/dist/AiurCore.min.js\"></script><script src=\"{_serviceLocation.UI}/dist/AiurDashboard.min.js\"></script></body></html>";
                    await context.Response.WriteAsync(text, Encoding.UTF8);

                    try
                    {
                        _logger.LogError(e, e.Message);
                        var accessToken = _appsContainer.AccessToken();
                        await _eventService.LogAsync(await accessToken, e.Message, e.StackTrace, EventLevel.Exception);
                    }
                    catch { }
                    return;
                }
                throw;
            }
        }
    }
}

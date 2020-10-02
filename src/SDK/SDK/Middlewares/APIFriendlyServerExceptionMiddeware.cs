using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.SDK.Services.ToStatusServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.SDK.Middlewares
{
    public class APIFriendlyServerExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<APIFriendlyServerExceptionMiddleware> _logger;
        private readonly AppsContainer _appsContainer;
        private readonly EventService _eventService;

        public APIFriendlyServerExceptionMiddleware(
            RequestDelegate next,
            ILogger<APIFriendlyServerExceptionMiddleware> logger,
            AppsContainer appsContainer,
            EventService eventService)
        {
            _next = next;
            _logger = logger;
            _appsContainer = appsContainer;
            _eventService = eventService;
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
                    context.Response.ContentType = "application/json; charset=utf-8";
                    var projectName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
                    var message = JsonConvert.SerializeObject(new AiurProtocol
                    {
                        Code = ErrorType.UnknownError,
                        Message = $"{projectName} server was crashed! Sorry about that."
                    });
                    await context.Response.WriteAsync(message, Encoding.UTF8);
                    try
                    {
                        _logger.LogError(e, e.Message);
                        var accessToken = _appsContainer.AccessToken();
                        await _eventService.LogExceptionAsync(await accessToken, e, context.Request.Path);
                    }
                    catch
                    {
                        // ignored
                    }

                    return;
                }
                throw;
            }
        }
    }
}

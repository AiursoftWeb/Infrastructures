using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Models;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aiursoft.SDK.Middlewares;

public class APIFriendlyServerExceptionMiddleware
{
    private readonly AppsContainer _appsContainer;
    private readonly EventService _eventService;
    private readonly ILogger<APIFriendlyServerExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

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
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json; charset=utf-8";
            var projectName = Assembly.GetEntryAssembly()?.GetName().Name;
            var message = JsonConvert.SerializeObject(new AiurProtocol
            {
                Code = ErrorType.UnknownError,
                Message = $"{projectName} server was crashed! Sorry about that."
            });
            await context.Response.WriteAsync(message, Encoding.UTF8);
            try
            {
                _logger.LogError(e, e.Message);
                var accessToken = await _appsContainer.AccessTokenAsync();
                await _eventService.LogExceptionAsync(accessToken, e, context.Request.Path);
            }
            catch
            {
                // ignored
            }
        }
    }
}
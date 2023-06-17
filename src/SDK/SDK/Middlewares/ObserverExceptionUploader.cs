using System;
using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aiursoft.SDK.Middlewares;

/// <summary>
/// This middleware will send the exception message in this app to Aiursoft Observer server.
/// </summary>
public class ObserverExceptionUploader
{
    private readonly DirectoryAppTokenService _directoryAppTokenService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ObserverExceptionUploader> _logger;
    private readonly RequestDelegate _next;

    public ObserverExceptionUploader(
        RequestDelegate next,
        ILogger<ObserverExceptionUploader> logger,
        DirectoryAppTokenService directoryAppTokenService,
        IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _logger = logger;
        _directoryAppTokenService = directoryAppTokenService;
        _scopeFactory = scopeFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception e)
        {
            try
            {
                _logger.LogCritical(e, "Observer exception handler detected a critical error in app and will try to upload to Observer server");
                var accessToken = await _directoryAppTokenService.GetAccessTokenAsync();
                var scope = _scopeFactory.CreateScope();
                var eventService = scope.ServiceProvider.GetRequiredService<ObserverService>();
                await eventService.LogExceptionAsync(accessToken, e, context.Request.Path);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to upload an exception to Observer server");
                throw e;
            }
        }
    }
}
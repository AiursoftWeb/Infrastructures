using Aiursoft.SDK.Middlewares;
using Aiursoft.CSTools.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.SDK;

public static class Handlers
{
    public static WebApplication UseAiursoftHandler(
        this WebApplication app, 
        bool allowCors = true, 
        bool addUserFriendlyPages = true)
    {
        // Recognize correct IP address.
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        // Allow CORS.
        if (allowCors)
        {
            app.UseCors();
        }

        // Use HTTPS redirection.
        app.UseHttpsRedirection();

        // Add error handlers.
        if (app.Environment.IsDevelopment() || !EntryExtends.IsProgramEntry())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Friendly page.
            app.UseMiddleware<ProductionExceptionPageMiddleware>();

            // Error tracking.
            app.UseMiddleware<ObserverExceptionUploader>();

            if (addUserFriendlyPages)
            {
                // User friendly pages.
                app.UseMiddleware<UserFriendlyNotFoundMiddleware>();
                app.UseMiddleware<UserFriendlyBadRequestMiddleware>();
            }
        }

        // Handle robots.
        app.UseMiddleware<HandleRobotsMiddleware>();

        return app;
    }
}
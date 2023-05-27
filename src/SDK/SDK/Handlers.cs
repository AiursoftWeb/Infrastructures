using Aiursoft.SDK.Middlewares;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace Aiursoft.SDK;

public static class Handlers
{
    public static IApplicationBuilder UseAiurUserHandler(this IApplicationBuilder app, bool isDevelopment)
    {
        if (isDevelopment || !EntryExtends.IsProgramEntry())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseMiddleware<HandleRobotsMiddleware>();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseHttpsRedirection();

            // TODO: Use Attributes
            app.UseMiddleware<UserFriendlyServerExceptionMiddleware>();
            app.UseMiddleware<UserFriendlyNotFoundMiddleware>();
            app.UseMiddleware<UserFriendlyBadRequestMiddleware>();
        }

        return app;
    }

    public static IApplicationBuilder UseAiurAPIHandler(this IApplicationBuilder app, bool isDevelopment)
    {
        if (isDevelopment || !EntryExtends.IsProgramEntry())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseMiddleware<HandleRobotsMiddleware>();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseHttpsRedirection();

            // TODO: Use Attributes
            app.UseMiddleware<APIFriendlyServerExceptionMiddleware>();
        }

        return app;
    }
}
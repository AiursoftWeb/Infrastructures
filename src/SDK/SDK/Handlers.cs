using Aiursoft.DocGenerator.Attributes;
using Aiursoft.DocGenerator.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner;
using Aiursoft.SDK.Attributes;
using Aiursoft.SDK.Middlewares;
using Aiursoft.SDK.Services;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Aiursoft.SDK
{
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
                app.UseMiddleware<EnforceHttpsMiddleware>();
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
                app.UseMiddleware<EnforceHttpsMiddleware>();
                app.UseMiddleware<APIFriendlyServerExceptionMiddleware>();
            }
            return app;
        }
    }
}

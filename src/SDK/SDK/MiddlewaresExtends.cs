using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.DocGenerator.Services;
using Aiursoft.SDK.Attributes;
using Aiursoft.SDK.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;

namespace Aiursoft.SDK;

public static class MiddlewaresExtends
{
    public static CultureInfo[] GetSupportedLanguages()
    {
        var supportedCultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("zh-CN")
        };
        return supportedCultures;
    }

    /// <summary>
    ///     Static files, routing, auth, language switcher, endpoints.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="beforeMvc"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseAiursoftAppRouters(
        this IApplicationBuilder app,
        Func<IApplicationBuilder,
            IApplicationBuilder> beforeMvc = null)
    {
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en"),
            SupportedCultures = GetSupportedLanguages(),
            SupportedUICultures = GetSupportedLanguages()
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = options =>
            {
                options.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(14)
                };
            }
        });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAiursoftAPIAppRouters(false, beforeMvc);
        app.UseMiddleware<SwitchLanguageMiddleware>();

        return app;
    }

    /// <summary>
    ///     Static files, routing, auth, language switcher, endpoints.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="addRouting"></param>
    /// <param name="beforeMvc"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseAiursoftAPIAppRouters(
        this IApplicationBuilder app,
        bool addRouting = true,
        Func<IApplicationBuilder, IApplicationBuilder> beforeMvc = null)
    {
        beforeMvc?.Invoke(app);
        if (addRouting)
        {
            app.UseRouting();
        }

        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        app.UseAiursoftDocGenerator(options =>
        {
            options.IsAPIAction = (action, controller) =>
            {
                return
                    action.CustomAttributes.Any(t => t.AttributeType == typeof(GenerateDoc)) ||
                    controller.CustomAttributes.Any(t => t.AttributeType == typeof(GenerateDoc)) ||
                    action.CustomAttributes.Any(t => t.AttributeType == typeof(ApiExceptionHandler)) ||
                    controller.CustomAttributes.Any(t => t.AttributeType == typeof(ApiExceptionHandler)) ||
                    action.CustomAttributes.Any(t => t.AttributeType == typeof(ApiModelStateChecker)) ||
                    controller.CustomAttributes.Any(t => t.AttributeType == typeof(ApiModelStateChecker));
            };
            options.RequiresAuthorized = (action, controller) =>
            {
                return
                    action.CustomAttributes.Any(t => t.AttributeType.IsAssignableFrom(typeof(IAiurForceAuth))) ||
                    controller.CustomAttributes.Any(t => t.AttributeType.IsAssignableFrom(typeof(IAiurForceAuth)));
            };
            options.Format = DocFormat.Json;
            options.GlobalApisPossibleResponses.Add(new AiurResponse
            {
                Code = Code.WrongKey,
                Message = "Some error."
            });
            options.GlobalApisPossibleResponses.Add(new AiurCollection<string>(new List<string> { "Some item is invalid!" })
            {
                Code = Code.InvalidInput,
                Message = "Your input contains several errors!"
            });
        });
        return app;
    }
}
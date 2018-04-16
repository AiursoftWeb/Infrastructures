using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToAPIServer;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Middlewares;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Microsoft.AspNetCore.Http;

namespace Aiursoft.Pylon
{
    public static class Extends
    {
        public static string CurrentAppId { get; private set; } = string.Empty;
        public static string CurrentAppSecret { get; private set; } = string.Empty;

        public static CultureInfo[] GetSupportedLanguages()
        {
            var SupportedCultures = new CultureInfo[]
            {
                new CultureInfo("en"),
                new CultureInfo("zh")
            };
            return SupportedCultures;
        }

        public static IApplicationBuilder UseAiursoftAuthenticationFromConfiguration(this IApplicationBuilder app, IConfiguration configuration, string appName)
        {
            var AppId = configuration[$"{appName}AppId"];
            var AppSecret = configuration[$"{appName}AppSecret"];
            Console.WriteLine($"Got AppId={AppId}, AppSecret={AppSecret}");
            if (string.IsNullOrWhiteSpace(AppId) || string.IsNullOrWhiteSpace(AppSecret))
            {
                throw new InvalidOperationException("Did not get appId and appSecret from configuration!");
            }
            return app.UseAiursoftAuthentication(AppId, AppSecret);
        }
        
        public static IApplicationBuilder UseAiursoftSupportedCultures(this IApplicationBuilder app, string defaultLanguage = "en")
        {
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultLanguage),
                SupportedCultures = GetSupportedLanguages(),
                SupportedUICultures = GetSupportedLanguages()
            });
            return app;
        }

        public static IApplicationBuilder UseAiursoftAuthentication(this IApplicationBuilder app, string appId, string appSecret)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new InvalidOperationException(nameof(appId));
            }
            if (string.IsNullOrWhiteSpace(appSecret))
            {
                throw new InvalidOperationException(nameof(appSecret));
            }
            CurrentAppId = appId;
            CurrentAppSecret = appSecret;
            return app;
        }

        public static IApplicationBuilder UseEnforceHttps(this IApplicationBuilder app)
        {
            return app.UseMiddleware<EnforceHttpsMiddleware>();
        }

        public static IApplicationBuilder UseHandleKahlaOptions(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HandleKahlaOptionsMiddleware>();
        }

        public static IApplicationBuilder UseLanguageSwitcher(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SwitchLanguageMiddleware>();
        }

        public static IServiceCollection ConfigureLargeFileUploadable(this IServiceCollection services)
        {
            return services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });
        }

        public static IActionResult SignoutRootServer(this Controller controller, AiurUrl ToRedirect)
        {
            var r = controller.HttpContext.Request;
            string serverPosition = $"{r.Scheme}://{r.Host}{ToRedirect}";
            var toRedirect = new AiurUrl(Values.ApiServerAddress, "oauth", "UserSignout", new UserSignoutAddressModel
            {
                ToRedirect = serverPosition
            });
            return controller.Redirect(toRedirect.ToString());
        }

        public static JsonResult Protocal(this Controller controller, ErrorType errorType, string errorMessage)
        {
            return controller.Json(new AiurProtocal
            {
                code = errorType,
                message = errorMessage
            });
        }
        
        public static void SetClientLang(this Controller controller, string culture)
        {
            controller.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }
    }
}

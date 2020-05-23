using Aiursoft.Gateway.SDK.Models;
using Aiursoft.Gateway.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Pylon.Middlewares;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.Authentication;
using Aiursoft.SDK;
using Aiursoft.SDK.Middlewares;
using Aiursoft.XelNaga.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Reflection;

namespace Aiursoft.Pylon
{
    public static class Extends
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

        public static IServiceCollection AddAiurMvc(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddAiurAPIMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            return services;
        }

        public static IApplicationBuilder UseAiurUserHandler(this IApplicationBuilder app, bool isDevelopment)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<HandleRobotsMiddleware>();
                app.UseMiddleware<EnforceHttpsMiddleware>();
                app.UseMiddleware<UserFriendlyServerExceptionMiddeware>();
                app.UseMiddleware<UserFriendlyNotFoundMiddeware>();
            }
            return app;
        }

        /// <summary>
        /// Static files, routing, auth, language switcher, endpoints.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAiursoftDefault(
            this IApplicationBuilder app,
            Func<IApplicationBuilder,
            IApplicationBuilder> beforeMVC = null)
        {
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = GetSupportedLanguages(),
                SupportedUICultures = GetSupportedLanguages()
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAiursoftAPIDefault(false, beforeMVC);
            app.UseMiddleware<SwitchLanguageMiddleware>();

            return app;
        }

        public static IActionResult SignOutRootServer(this Controller controller, string apiServerAddress, AiurUrl viewingUrl)
        {
            var request = controller.HttpContext.Request;
            string serverPosition = $"{request.Scheme}://{request.Host}{viewingUrl}";
            var toRedirect = new AiurUrl(apiServerAddress, "OAuth", "UserSignout", new UserSignoutAddressModel
            {
                ToRedirect = serverPosition
            });
            return controller.Redirect(toRedirect.ToString());
        }

        public static void SetClientLang(this Controller controller, string culture)
        {
            controller.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }

        public static IServiceCollection AddAiurDependenciesWithIdentity<TUser>(this IServiceCollection services) where TUser : AiurUserBase, new()
        {
            if (Assembly.GetEntryAssembly().FullName.StartsWith("ef"))
            {
                Console.WriteLine("Calling from Entity Framework! Skipped dependencies management!");
                return services;
            }
            services.AddAiurDependencies(abstracts: typeof(IAuthProvider));
            services.AddScoped<UserImageGenerator<TUser>>();
            services.AddScoped<AuthService<TUser>>();
            return services;
        }
    }
}

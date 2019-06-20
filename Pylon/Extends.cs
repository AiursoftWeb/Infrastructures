using Aiursoft.Pylon.Middlewares;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.API.OAuthAddressModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToAPIServer;
using Aiursoft.Pylon.Services.ToArchonServer;
using Aiursoft.Pylon.Services.ToOSSServer;
using Aiursoft.Pylon.Services.ToProbeServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Globalization;

namespace Aiursoft.Pylon
{
    public static class Extends
    {
        public static string CurrentAppId { get; private set; } = string.Empty;
        public static string CurrentAppSecret { get; private set; } = string.Empty;

        private static CultureInfo[] GetSupportedLanguages()
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("zh")
            };
            return supportedCultures;
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

        public static IApplicationBuilder UseAiursoftAuthenticationFromConfiguration(this IApplicationBuilder app, IConfiguration configuration, string appName)
        {
            var appId = configuration[$"{appName}AppId"];
            var appSecret = configuration[$"{appName}AppSecret"];
            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(appSecret))
            {
                throw new InvalidOperationException("Did not get appId and appSecret from configuration!");
            }
            return app.UseAiursoftAuthentication(appId, appSecret);
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

        public static IApplicationBuilder UseHandleRobots(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HandleRobotsMiddleware>();
        }

        public static IApplicationBuilder UseLanguageSwitcher(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SwitchLanguageMiddleware>();
        }

        public static IApplicationBuilder UseDocGenerator(this IApplicationBuilder app)
        {
            return app.UseMiddleware<APIDocGeneratorMiddleware>();
        }

        public static IServiceCollection ConfigureLargeFileUpload(this IServiceCollection services)
        {
            return services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });
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

        public static JsonResult Protocol(this Controller controller, ErrorType errorType, string errorMessage)
        {
            return controller.Json(new AiurProtocol
            {
                Code = errorType,
                Message = errorMessage
            });
        }

        public static void SetClientLang(this Controller controller, string culture)
        {
            controller.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder = null) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                var configuration = services.GetService<IConfiguration>();
                var env = services.GetService<IHostingEnvironment>();

                var connectionString = configuration.GetConnectionString("DatabaseConnection");
                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
                    logger.LogInformation($"Connection string is {connectionString}");
                    var retry = Policy.Handle<Exception>().WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(15),
                    });

                    retry.Execute(() =>
                    {
                        // Migrate even in production level.
                        context.Database.Migrate();
                        if (env.IsDevelopment())
                        {
                            try
                            {
                                seeder?.Invoke(context, services);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, $"An error occurred while seeding the database used on context {typeof(TContext).Name}");
                            }
                        }
                    });
                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return webHost;
        }

        public static IServiceCollection AddAiursoftAuth<TUser>(this IServiceCollection services) where TUser : AiurUserBase, new()
        {
            services.AddSingleton<AppsContainer>();
            services.AddSingleton<ServiceLocation>();
            services.AddScoped<ArchonApiService>();
            services.AddScoped<HTTPService>();
            services.AddScoped<UrlConverter>();
            services.AddScoped<OSSApiService>();
            services.AddScoped<SitesService>();
            services.AddScoped<FoldersService>();
            services.AddScoped<StorageService>();
            services.AddScoped<CoreApiService>();
            services.AddScoped<AccountService>();
            services.AddScoped<UserImageGenerator<TUser>>();
            services.AddTransient<AuthService<TUser>>();
            return services;
        }

        public static IServiceCollection AddTokenManager(this IServiceCollection services)
        {
            services.AddSingleton<AiurKeyPair>();
            services.AddTransient<RSAService>();
            services.AddTransient<ACTokenManager>();
            return services;
        }
    }
}

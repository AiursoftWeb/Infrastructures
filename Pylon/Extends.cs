using Aiursoft.Archon.SDK;
using Aiursoft.Developer.SDK;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.DocGenerator.Services;
using Aiursoft.Gateway.SDK;
using Aiursoft.Gateway.SDK.Models;
using Aiursoft.Gateway.SDK.Models.API.OAuthAddressModels;
using Aiursoft.Handler.Abstract.Models;
using Aiursoft.Handler.Attributes;
using Aiursoft.Probe.SDK;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Middlewares;
using Aiursoft.Pylon.Services;
using Aiursoft.Scanner;
using Aiursoft.Stargate.SDK;
using Aiursoft.Status.SDK;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Aiursoft.Pylon
{
    public static class Extends
    {
        private static CultureInfo[] GetSupportedLanguages()
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("zh")
            };
            return supportedCultures;
        }

        public static IServiceCollection AddAiurMvc(this IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services
                .AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            return services;
        }

        public static IApplicationBuilder UseAiurAPIHandler(this IApplicationBuilder app, bool isDevelopment)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseMiddleware<HandleRobotsMiddleware>();
                app.UseMiddleware<EnforceHttpsMiddleware>();
                app.UseMiddleware<APIFriendlyServerExceptionMiddeware>();
            }
            return app;
        }

        public static IApplicationBuilder UseAiurUserHandler(this IApplicationBuilder app, bool isDevelopment)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
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
            beforeMVC?.Invoke(app);
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
            app.UseMiddleware<SwitchLanguageMiddleware>();
            app.UseAiursoftDocGenerator(options =>
            {
                options.IsAPIAction = (action, controller) =>
                {
                    return
                        action.CustomAttributes.Any(t => t.AttributeType == typeof(GenerateDoc)) ||
                        controller.CustomAttributes.Any(t => t.AttributeType == typeof(GenerateDoc)) ||
                        action.CustomAttributes.Any(t => t.AttributeType == typeof(APIExpHandler)) ||
                        controller.CustomAttributes.Any(t => t.AttributeType == typeof(APIExpHandler)) ||
                        action.CustomAttributes.Any(t => t.AttributeType == typeof(APIModelStateChecker)) ||
                        controller.CustomAttributes.Any(t => t.AttributeType == typeof(APIModelStateChecker));
                };
                options.JudgeAuthorized = (action, controller) =>
                {
                    return
                        action.CustomAttributes.Any(t => t.AttributeType == typeof(AiurForceAuth)) ||
                        controller.CustomAttributes.Any(t => t.AttributeType == typeof(AiurForceAuth));
                };
                options.Format = DocFormat.Json;
                options.GlobalPossibleResponse.Add(new AiurProtocol
                {
                    Code = ErrorType.WrongKey,
                    Message = "Some error."
                });
                options.GlobalPossibleResponse.Add(new AiurCollection<string>(new List<string> { "Some item is invalid!" })
                {
                    Code = ErrorType.InvalidInput,
                    Message = "Your input contains several errors!"
                });
            });
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

        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder = null) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                var configuration = services.GetService<IConfiguration>();
                var env = services.GetService<IWebHostEnvironment>();

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
                        try
                        {
                            seeder?.Invoke(context, services);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"An error occurred while seeding the database used on context {typeof(TContext).Name}");
                        }
                    });
                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return host;
        }

        public static IServiceCollection AddAiurDependenciesWithIdentity<TUser>(this IServiceCollection services) where TUser : AiurUserBase, new()
        {
            if (Assembly.GetEntryAssembly().FullName.StartsWith("ef"))
            {
                Console.WriteLine("Calling from Entity Framework! Skipped dependencies management!");
                return services;
            }
            services.AddAiurDependencies();
            services.AddScoped<UserImageGenerator<TUser>>();
            services.AddScoped<AuthService<TUser>>();
            return services;
        }

        public static IServiceCollection AddAiurDependencies(this IServiceCollection services)
        {
            // Use status server to report bugs.
            services.AddStatusServer();
            services.AddArchonServer();
            services.AddStargateServer();
            services.AddProbeServer();
            services.AddDeveloperServer();
            services.AddGatewayServer();
            services.AddHttpClient();
            services.AddMemoryCache();
            if (Assembly.GetEntryAssembly().FullName.StartsWith("ef"))
            {
                Console.WriteLine("Calling from Entity Framework! Skipped dependencies management!");
                return services;
            }
            services.AddScannedDependencies(typeof(IHostedService), typeof(IAuthProvider));
            return services;
        }
    }
}

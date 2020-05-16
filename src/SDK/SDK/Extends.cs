using Aiursoft.Archon.SDK;
using Aiursoft.Developer.SDK;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.DocGenerator.Services;
using Aiursoft.Gateway.SDK;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.SDK;
using Aiursoft.Scanner;
using Aiursoft.SDK.Attributes;
using Aiursoft.SDK.Middlewares;
using Aiursoft.SDK.Services.Authentication;
using Aiursoft.Stargate.SDK;
using Aiursoft.Status.SDK;
using Aiursoft.XelNaga.Services;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aiursoft.SDK
{
    public static class Extends
    {
        public static IMvcBuilder AddAiurAPIMvc(this IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            return services
                .AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }

        public static IApplicationBuilder UseAiurAPIHandler(this IApplicationBuilder app, bool isDevelopment)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<HandleRobotsMiddleware>();
                app.UseMiddleware<EnforceHttpsMiddleware>();
                app.UseMiddleware<APIFriendlyServerExceptionMiddeware>();
            }
            return app;
        }

        /// <summary>
        /// Static files, routing, auth, language switcher, endpoints.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAiursoftAPIDefault(
            this IApplicationBuilder app,
            bool addRouting = true,
            Func<IApplicationBuilder, IApplicationBuilder> beforeMVC = null)
        {
            beforeMVC?.Invoke(app);
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
                        action.CustomAttributes.Any(t => t.AttributeType == typeof(APIExpHandler)) ||
                        controller.CustomAttributes.Any(t => t.AttributeType == typeof(APIExpHandler)) ||
                        action.CustomAttributes.Any(t => t.AttributeType == typeof(APIModelStateChecker)) ||
                        controller.CustomAttributes.Any(t => t.AttributeType == typeof(APIModelStateChecker));
                };
                options.JudgeAuthorized = (action, controller) =>
                {
                    return
                        action.CustomAttributes.Any(t => t.AttributeType.IsAssignableFrom(typeof(IAiurForceAuth))) ||
                        controller.CustomAttributes.Any(t => t.AttributeType.IsAssignableFrom(typeof(IAiurForceAuth)));
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

        public static IServiceCollection AddAiurDependencies(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMemoryCache();
            // Use status server to report bugs.
            services.AddStatusServer();
            services.AddArchonServer();
            services.AddStargateServer();
            services.AddProbeServer();
            services.AddDeveloperServer();
            services.AddGatewayServer();
            if (Assembly.GetEntryAssembly().FullName.StartsWith("ef"))
            {
                Console.WriteLine("Calling from Entity Framework! Skipped dependencies management!");
                return services;
            }
            services.AddScannedDependencies(typeof(IHostedService), typeof(IAuthProvider));
            return services;
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
                    AsyncHelper.TryAsyncThreeTimes(async () =>
                    {
                        await context.Database.MigrateAsync();
                        seeder?.Invoke(context, services);
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

        public static IServiceCollection AddDbContextWithCache<T>(this IServiceCollection services, string connectionString) where T : DbContext
        {
            services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
                    optionsBuilder
                        .UseSqlServer(connectionString)
                        .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
            services.AddEFSecondLevelCache(options =>
            {
                options.UseMemoryCacheProvider().DisableLogging(true);
                options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
            });
            return services;
        }
    }
}

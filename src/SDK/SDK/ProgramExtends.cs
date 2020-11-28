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
    public static class ProgramExtends
    {
        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder = null) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();
            var configuration = services.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DatabaseConnection");
            try
            {
                logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
                logger.LogInformation($"Connection string is {connectionString}");
                AsyncHelper.TryAsync(async () =>
                {
                    await context.Database.MigrateAsync();
                    seeder?.Invoke(context, services);
                }, 3, (e) =>
                {
                    logger.LogCritical(e, "Seed database failed.");
                });
                logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
            }

            return host;
        }
    }
}

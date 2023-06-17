using System;
using System.Linq;
using System.Reflection;
using Aiursoft.Scanner;
using Aiursoft.XelNaga.Tools;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aiursoft.SDK;

public static class ServicesExtends
{
    public static IServiceCollection AddAiurMvc(this IServiceCollection services)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        // TODO: Use it as an attribute to only apply to API.
        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

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

    public static IServiceCollection AddAiursoftSdk(this IServiceCollection services,
        Assembly assembly = null,
        params Type[] abstracts)
    {
        services.AddHttpClient();
        services.AddMemoryCache();
        var abstractsList = abstracts.ToList();
        abstractsList.Add(typeof(IHostedService));
        if (EntryExtends.IsProgramEntry())
            // Program is starting itself.
        {
            services.AddScannedDependencies(abstractsList.ToArray());
        }
        else if (assembly != null)
            // Program is started in UT or EF. Method called from extension.
        {
            services.AddAssemblyDependencies(assembly, abstractsList.ToArray());
        }
        else
            // Program is started in UT or EF. Method called from web project.
        {
            services.AddAssemblyDependencies(Assembly.GetCallingAssembly(), abstractsList.ToArray());
        }

        return services;
    }

    public static IServiceCollection AddDbContextWithCache<T>(this IServiceCollection services, string connectionString)
        where T : DbContext
    {
        if (EntryExtends.IsInUT())
        {
            services.AddDbContext<T>((serviceProvider, optionsBuilder) =>
                optionsBuilder
                    .UseInMemoryDatabase("inmemory")
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        }
        // Consider some new EF technology like use memory cache.
        else
        {
            services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
                optionsBuilder
                    .UseSqlServer(connectionString)
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
        }

        services.AddEFSecondLevelCache(options =>
        {
            options.UseMemoryCacheProvider().DisableLogging(true);
            options.CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30));
        });
        return services;
    }
}
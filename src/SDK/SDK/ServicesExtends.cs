using Aiursoft.DocGenerator.Attributes;
using Aiursoft.DocGenerator.Services;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner;
using Aiursoft.Scanner.Tools;
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
    public static class ServicesExtends
    {
        public static IServiceCollection AddAiurMvc(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddAiurAPIMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            return services;
        }

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

        public static IServiceCollection AddAiursoftSDK(this IServiceCollection services, params Type[] abstracts)
        {
            services.AddHttpClient();
            services.AddMemoryCache();
            if (EntryExtends.IsProgramEntry())
            {
                services.AddScannedDependencies(abstracts.AddWith(typeof(IHostedService)).ToArray());
            }
            else
            {
                services.AddLibraryDependencies(abstracts.AddWith(typeof(IHostedService)).ToArray());
            }
            return services;
        }

        public static IServiceCollection UseBlacklistFromAddress(this IServiceCollection services, string address)
        {
            AsyncHelper.TryAsync(async () =>
            {
                var list = await new WebClient().DownloadStringTaskAsync(address);
                var provider = new BlackListPorivder(list.Split('\n'));
                services.AddSingleton(provider);
            }, 3);
            return services;
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

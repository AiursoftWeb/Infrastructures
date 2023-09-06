using System.Reflection;
using Aiursoft.AiurProtocol.Server;
using Aiursoft.Scanner;
using Aiursoft.CSTools.Tools;
using Aiursoft.DbTools.InMemory;
using Aiursoft.DbTools.SqlServer;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.SDK;

public static class ServicesExtends
{
    /// <summary>
    /// This method will add all the features that Aiursoft web services need. Including:
    ///  - CORS
    ///  - Localization
    ///  - AiurProtocol
    ///  - ViewLocalization
    ///  - DataAnnotationsLocalization
    ///  - View Components
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IMvcBuilder AddAiursoftWebFeatures(this IServiceCollection services)
    {
        // TODO: Use it as an attribute to only apply to API.
        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        services.AddLocalization(options => options.ResourcesPath = "Resources");
        var mvcBuilder = services
            .AddControllersWithViews()
            .AddApplicationPart(Assembly.GetCallingAssembly())
            .AddApplicationPart(Assembly.GetExecutingAssembly())
            .AddAiurProtocol()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        return mvcBuilder;
    }

    public static IServiceCollection AddScannedServices(this IServiceCollection services,
        Assembly callingAssembly = null,
        params Type[] abstracts)
    {
        services.AddHttpClient();
        services.AddMemoryCache();
        var abstractsList = abstracts.ToList();
        abstractsList.Add(typeof(IHostedService));
        if (EntryExtends.IsProgramEntry()) // Program is starting itself.
        {
            services.AddScannedDependencies(abstractsList.ToArray());
        }
        else
        {
            if (callingAssembly == null)
            {
                callingAssembly = Assembly.GetCallingAssembly();
            }
            services.AddAssemblyDependencies(callingAssembly, abstractsList.ToArray());
        }

        return services;
    }

    public static IServiceCollection AddDbContextForInfraApps<T>(this IServiceCollection services, string connectionString)
        where T : DbContext
    {
        if (EntryExtends.IsInUnitTests())
        {
            services.AddAiurInMemoryDb<T>();
        }
        else
        {
            services.AddAiurSqlServerWithCache<T>(connectionString);
        }
        return services;
    }
}
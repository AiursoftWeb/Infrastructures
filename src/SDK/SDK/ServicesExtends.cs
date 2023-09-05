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
    public static IServiceCollection AddAiurMvc(this IServiceCollection services)
    {
        // TODO: Use it as an attribute to only apply to API.
        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services
            .AddControllersWithViews()
            .AddApplicationPart(Assembly.GetCallingAssembly())
            .AddAiurProtocol()
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
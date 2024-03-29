﻿using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.WebTools.Abstractions.Models;
using Aiursoft.WWW.Data;
using Aiursoft.WWW.Models;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.WWW;

public class Startup : IWebStartup
{
    public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        services.AddDbContextForInfraApps<WWWDbContext>(configuration.GetConnectionString("DatabaseConnection"));
        services.Configure<List<Navbar>>(configuration.GetSection("Navbar"));

        services.AddIdentity<WWWUser, IdentityRole>()
            .AddEntityFrameworkStores<WWWDbContext>()
            .AddDefaultTokenProviders();
        services.AddAiursoftWebFeatures();
        services.AddAiursoftIdentity<WWWUser>(
            probeConfig: configuration.GetSection("AiursoftProbe"),
            authenticationConfig: configuration.GetSection("AiursoftAuthentication"),
            observerConfig: configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(WebApplication app)
    {
        app.UseAiursoftHandler();
        app.UseAiursoftAppRouters();
    }
}
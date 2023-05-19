using System.Collections.Generic;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.WWW.Data;
using Aiursoft.WWW.Models;
using Aiursoft.WWW.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.WWW;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        AppsContainer.CurrentAppId = configuration["WWWAppId"];
        AppsContainer.CurrentAppSecret = configuration["WWWAppSecret"];
        BingTranslator.APIKey = configuration["TranslateAPIKey"];
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextWithCache<WWWDbContext>(Configuration.GetConnectionString("DatabaseConnection"));
        services.Configure<List<Navbar>>(Configuration.GetSection("Navbar"));

        services.AddIdentity<WWWUser, IdentityRole>()
            .AddEntityFrameworkStores<WWWDbContext>()
            .AddDefaultTokenProviders();
        services.UseBlacklistFromAddress(Configuration["BlackListLocation"]);
        services.AddAiurMvc();
        services.AddAiursoftIdentity<WWWUser>(
            Configuration.GetConnectionString("ObserverConnection"),
            Configuration.GetConnectionString("ProbeConnection"),
            Configuration.GetConnectionString("GatewayConnection"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiurUserHandler(env.IsDevelopment());
        app.UseAiursoftDefault();
    }
}
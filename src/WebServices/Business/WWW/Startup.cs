using System.Collections.Generic;
using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.WWW.Data;
using Aiursoft.WWW.Models;
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
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextWithCache<WWWDbContext>(Configuration.GetConnectionString("DatabaseConnection"));
        services.Configure<List<Navbar>>(Configuration.GetSection("Navbar"));

        services.AddIdentity<WWWUser, IdentityRole>()
            .AddEntityFrameworkStores<WWWDbContext>()
            .AddDefaultTokenProviders();
        services.AddAiurMvc();
        services.AddAiursoftIdentity<WWWUser>(
            probeConfig: Configuration.GetSection("AiursoftProbe"),
            authenticationConfig: Configuration.GetSection("AiursoftAuthentication"),
            observerConfig: Configuration.GetSection("AiursoftObserver"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAiursoftHandler(env.IsDevelopment());
        app.UseAiursoftAppRouters();
    }
}
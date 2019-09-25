using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToArchonServer;
using Aiursoft.Pylon.Services.ToStargateServer;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Stargate
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddDbContext<StargateDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services
                .AddControllersWithViews()
                .AddNewtonsoftJson();

            services.AddTokenManager();

            services.AddSingleton<ServiceLocation>();
            services.AddSingleton<IHostedService, TimedCleaner>();
            services.AddSingleton<Counter>();
            services.AddSingleton<AppsContainer>();
            services.AddSingleton<StargateMemory>();
            services.AddSingleton<TimeoutCleaner>();
            services.AddScoped<ArchonApiService>();
            services.AddScoped<HTTPService>();
            services.AddScoped<ChannelService>();
            services.AddScoped<PushMessageService>();

            services.AddScoped<Debugger>();
            services.AddScoped<IPusher, WebSocketPusher>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHandleRobots();
                app.UseEnforceHttps();
                app.UseAPIFriendlyErrorPage();
            }
            app.UseAiursoftAuthenticationFromConfiguration(Configuration, "Test");
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
            app.UseDocGenerator();
        }
    }
}

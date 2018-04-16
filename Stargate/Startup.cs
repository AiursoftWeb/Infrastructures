using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Aiursoft.Stargate.Data;
using Aiursoft.Stargate.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using Microsoft.Extensions.Hosting;
using Aiursoft.Pylon.Services.ToStargateServer;

namespace Aiursoft.Stargate
{
    public class Startup
    {
        public static Counter MessageIdCounter { get; set; } = new Counter();
        public static Counter ListenerIdCounter { get; set; } = new Counter();
        public static Random RandomObject { get; set; } = new Random();

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StargateDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));
            services.AddMvc();
            services.AddTransient<HTTPService>();
            services.AddTransient<PushMessageService>();
            services.AddTransient<Debugger>();
            services.AddTransient<WebSocketPusher>();
            services.AddSingleton<IHostedService, TimedCleaner>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseEnforceHttps();
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseWebSockets();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseLanguageSwitcher();
            app.UseAiursoftAuthenticationFromConfiguration(Configuration, "Test");
        }
    }
}

using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Archon
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services
                .AddControllersWithViews()
                .AddNewtonsoftJson();

            services.AddTokenManager();
            services.AddSingleton<ServiceLocation>();
            services.AddHttpClient();
            services.AddScoped<HTTPService>();
            services.AddScoped<DeveloperApiService>();
            services.AddTransient<AiurCache>();
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
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
            app.UseDocGenerator();
        }
    }
}

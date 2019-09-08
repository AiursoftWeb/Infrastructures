using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Archon
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTokenManager();
            services.AddSingleton<ServiceLocation>();
            services.AddScoped<HTTPService>();
            services.AddScoped<DeveloperApiService>();
            services.AddMemoryCache();
            services.AddTransient<AiurCache>();
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
                app.UseHandleRobots();
                app.UseEnforceHttps();
                app.UseAPIFriendlyErrorPage();
            }
            app.UseMvcWithDefaultRoute();
            app.UseDocGenerator();
        }
    }
}

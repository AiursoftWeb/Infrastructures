using Aiursoft.Probe.Data;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToAPIServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Probe
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
            services.ConfigureLargeFileUpload();
            services.AddDbContext<ProbeDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services.AddTokenManager();
            services.AddSingleton<ServiceLocation>();
            services.AddSingleton<IHostedService, TimedCleaner>();
            services.AddScoped<HTTPService>();
            services.AddScoped<CoreApiService>();
            services.AddTransient<ImageCompressor>();
            services.AddTransient<FolderLocator>();
            services.AddTransient<FolderCleaner>();
            services.AddTransient<FolderRefactor>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
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
                app.UseExceptionHandler("/Error/ServerException");
                app.UseStatusCodePagesWithReExecute("/Error/Code{0}");
            }
            app.UseCors(builder => builder.WithOrigins("*"));
            app.UseMvcWithDefaultRoute();
            app.UseDocGenerator();
        }
    }
}

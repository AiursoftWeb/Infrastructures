using Aiursoft.Probe.Data;
using Aiursoft.Probe.Middlewares;
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
            services.AddApplicationInsightsTelemetry();

            //services.Configure<FormOptions>(x => x.MultipartBodyLengthLimit = long.MaxValue);

            services.AddDbContext<ProbeDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services
                .AddControllersWithViews()
                .AddNewtonsoftJson();

            services.AddTokenManager();
            services.AddSingleton<ServiceLocation>();
            services.AddSingleton<IHostedService, TimedCleaner>();
            services.AddSingleton<PBKeyPair>();
            services.AddHttpClient();
            services.AddScoped<HTTPService>();
            services.AddScoped<CoreApiService>();
            services.AddTransient<PBRSAService>();
            services.AddTransient<PBTokenManager>();
            services.AddTransient<ImageCompressor>();
            services.AddTransient<FolderLocator>();
            services.AddTransient<FolderOperator>();
            services.AddTransient<FolderRefactor>();
            services.AddTransient<AiurCache>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ProbeCORSMiddleware>();
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

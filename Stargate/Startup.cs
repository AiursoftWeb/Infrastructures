using Aiursoft.Pylon;
using Aiursoft.Stargate.Data;
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
            services.AddDbContext<StargateDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services.AddAiurMvc();

            services.AddAiurDependencies("Test");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurAPIHandler(env.IsDevelopment());
            app.UseWebSockets();
            app.UseAiursoftDefault();
        }
    }
}

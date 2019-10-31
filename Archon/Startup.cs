using Aiursoft.Pylon;
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
            services.AddAiurMvc();
            services.AddAiurDependencies("Archon");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurAPIHandler(env.IsDevelopment());
            app.UseAiursoftDefault();
        }
    }
}

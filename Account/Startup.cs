using Aiursoft.Account.Data;
using Aiursoft.Account.Models;
using Aiursoft.Pylon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Account
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
            services.AddDbContext<AccountDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services.AddIdentity<AccountUser, IdentityRole>()
                .AddEntityFrameworkStores<AccountDbContext>()
                .AddDefaultTokenProviders();

            services.AddAiurMvc();

            services.AddAiurDependencies<AccountUser>("Account");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiurUserHandler(env.IsDevelopment());
            app.UseAiursoftDefault();
        }
    }
}

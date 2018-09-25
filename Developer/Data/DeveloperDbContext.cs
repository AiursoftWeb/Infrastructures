using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Models.Developer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon;
using System.Threading.Tasks;
using Aiursoft.Pylon.Models;
using Microsoft.Extensions.Logging;
using Aiursoft.Pylon.Services;

namespace Aiursoft.Developer.Data
{
    public class DeveloperDbContext : IdentityDbContext<DeveloperUser>
    {
        public DeveloperDbContext(DbContextOptions<DeveloperDbContext> options)
            : base(options)
        {
        }

        public DbSet<App> Apps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public void Seed(IServiceProvider services)
        {
            var config = services.GetService<IConfiguration>();
            var usermanager = services.GetService<UserManager<DeveloperUser>>();
            var serviceLocation = services.GetService<ServiceLocation>();
            var logger = services.GetRequiredService<ILogger<DeveloperDbContext>>();

            var firstUserName = config["SeedUserEmail"];
            var firstUserPass = config["SeedUserPassword"];

            var firstAppId = config["DeveloperAppId"];
            var firstAppSecret = config["DeveloperAppSecret"];


            logger.LogInformation("Seeding developer database...");
            var newuser = new DeveloperUser
            {
                Id = "01307818-4d2d-43d1-acde-c91e333b3ade",
                UserName = firstUserName,
                Email = firstUserName,
                NickName = "Demo User",
                PreferedLanguage = "en",
                HeadImgFileKey = Values.DefaultImageId
            };
            usermanager.CreateAsync(newuser, firstUserPass).Wait();

            var newApp = new App("Developer", "Seeded developer app", Category.AppForAiur, Platform.Web, firstAppId, firstAppSecret)
            {
                CreatorId = newuser.Id,
                AppIconAddress = $"{serviceLocation.CDN}/images/appdefaulticon.png"
            };
            this.Apps.Add(newApp);
            this.SaveChanges();

            logger.LogInformation("the developer database was successfully seeded!");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.API.Models;
using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon;
using Microsoft.Extensions.Logging;

namespace Aiursoft.API.Data
{
    public class APIDbContext : IdentityDbContext<APIUser>
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
        }

        public DbSet<AccessToken> AccessToken { get; set; }
        public DbSet<OAuthPack> OAuthPack { get; set; }
        public DbSet<AppGrant> LocalAppGrant { get; set; }
        public DbSet<UserEmail> UserEmails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public void SyncEmail()
        {
            var users = Users.Include(t => t.Emails).ToList();
            foreach (var user in users)
            {
                bool hasPrimaryEmail = user.Emails.Exists(t => t.EmailAddress == user.Email.ToLower());
                if (user.Emails.Count() == 0 || !hasPrimaryEmail)
                {
                    var primaryEmail = new UserEmail
                    {
                        OwnerId = user.Id,
                        EmailAddress = user.Email.ToLower()
                    };
                    UserEmails.Add(primaryEmail);
                    SaveChanges();
                }
            }
        }

        public void Seed(IServiceProvider services)
        {
            var config = services.GetService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<APIDbContext>>();
            var usermanager = services.GetService<UserManager<APIUser>>();
            var serviceLocation = services.GetService<ServiceLocation>();

            var firstUserName = config["SeedUserEmail"];
            var firstUserPass = config["SeedUserPassword"];

            logger.LogInformation("Seeding database to API app.");

            var newuser = new APIUser
            {
                Id = "01307818-4d2d-43d1-acde-c91e333b3ade",
                UserName = firstUserName,
                Email = firstUserName,
                NickName = "Demo User",
                PreferedLanguage = "en",
                HeadImgUrl = $"{serviceLocation.CDN}/images/userdefaulticon.png"
            };
            usermanager.CreateAsync(newuser, firstUserPass).Wait();

            var primaryMail = new UserEmail
            {
                EmailAddress = newuser.Email.ToLower(),
                OwnerId = newuser.Id
            };
            this.UserEmails.Add(primaryMail);
            
            this.SaveChanges();
            logger.LogInformation("Successfully seeded user to API!");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.API.Models;
using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models;

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
    }
}

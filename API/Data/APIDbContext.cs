using Aiursoft.API.Models;
using Aiursoft.Pylon.Models.API;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.API.Data
{
    public class APIDbContext : IdentityDbContext<APIUser>
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
        }

        public DbSet<OAuthPack> OAuthPack { get; set; }
        public DbSet<AppGrant> LocalAppGrant { get; set; }
        public DbSet<UserEmail> UserEmails { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}

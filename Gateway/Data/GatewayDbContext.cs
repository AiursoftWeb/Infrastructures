using Aiursoft.Gateway.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Gateway.Data
{
    public class GatewayDbContext : IdentityDbContext<GatewayUser>
    {
        public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options)
        {
        }

        public DbSet<OAuthPack> OAuthPack { get; set; }
        public DbSet<AppGrant> LocalAppGrant { get; set; }
        public DbSet<UserEmail> UserEmails { get; set; }
        public DbSet<AuditLogLocal> AuditLogs { get; set; }
    }
}

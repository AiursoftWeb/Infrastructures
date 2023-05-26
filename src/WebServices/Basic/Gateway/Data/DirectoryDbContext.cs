using Aiursoft.Directory.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Directory.Data;

public class DirectoryDbContext : IdentityDbContext<DirectoryUser>
{
    public DirectoryDbContext(DbContextOptions<DirectoryDbContext> options) : base(options)
    {
    }

    public DbSet<OAuthPack> OAuthPack { get; set; }
    public DbSet<AppGrant> LocalAppGrant { get; set; }
    public DbSet<UserEmail> UserEmails { get; set; }
    public DbSet<AuditLogLocal> AuditLogs { get; set; }
    public DbSet<ThirdPartyAccount> ThirdPartyAccounts { get; set; }
}
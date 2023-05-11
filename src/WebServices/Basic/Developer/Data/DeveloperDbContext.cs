using Aiursoft.Developer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Developer.Data;

public class DeveloperDbContext : IdentityDbContext<DeveloperUser>
{
    public DeveloperDbContext(DbContextOptions<DeveloperDbContext> options)
        : base(options)
    {
    }

    public DbSet<DeveloperApp> Apps { get; set; }
}
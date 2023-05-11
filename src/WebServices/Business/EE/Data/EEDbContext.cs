using Aiursoft.EE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.EE.Data;

public class EEDbContext : IdentityDbContext<EEUser>
{
    public EEDbContext(DbContextOptions<EEDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Follow> Follows { get; set; }
}
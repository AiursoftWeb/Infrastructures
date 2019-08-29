using Aiursoft.Pylon.Models.Developer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Developer.Data
{
    public class DeveloperDbContext : IdentityDbContext<DeveloperUser>
    {
        public DeveloperDbContext(DbContextOptions<DeveloperDbContext> options)
            : base(options)
        {
        }

        public DbSet<App> Apps { get; set; }
    }
}

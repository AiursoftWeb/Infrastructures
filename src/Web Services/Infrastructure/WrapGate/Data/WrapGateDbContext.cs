using Aiursoft.Wrapgate.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Wrapgate.Data
{
    public class WrapgateDbContext : DbContext
    {
        public WrapgateDbContext(DbContextOptions<WrapgateDbContext> options) : base(options)
        {
        }

        public DbSet<WrapgateApp> WrapApps { get; set; }
        public DbSet<WrapRecord> Records { get; set; }
    }
}

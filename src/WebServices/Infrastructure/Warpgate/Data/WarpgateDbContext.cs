using Aiursoft.Warpgate.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warpgate.Data
{
    public class WarpgateDbContext : DbContext
    {
        public WarpgateDbContext(DbContextOptions<WarpgateDbContext> options) : base(options)
        {
        }

        public DbSet<WarpgateApp> WarpApps { get; set; }
        public DbSet<WarpRecord> Records { get; set; }
    }
}

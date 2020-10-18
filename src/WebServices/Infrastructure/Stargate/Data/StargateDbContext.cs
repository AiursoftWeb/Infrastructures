using Aiursoft.Stargate.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Stargate.Data
{
    public class StargateDbContext : DbContext
    {
        public StargateDbContext(DbContextOptions<StargateDbContext> options) : base(options)
        {
        }

        public DbSet<StargateApp> Apps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

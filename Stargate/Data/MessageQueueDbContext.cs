using Aiursoft.Pylon.Models.Stargate;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Stargate.Data
{
    public class StargateDbContext : DbContext
    {
        public StargateDbContext(DbContextOptions<StargateDbContext> options) : base(options)
        {
        }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<StargateApp> Apps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

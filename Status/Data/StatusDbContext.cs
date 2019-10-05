using Aiursoft.Pylon.Models.Stargate;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Status.Data
{
    public class StatusDbContext : DbContext
    {
        public StatusDbContext(DbContextOptions<StatusDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

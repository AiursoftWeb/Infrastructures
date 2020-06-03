using Aiursoft.WrapGate.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.WrapGate.Data
{
    public class WrapGateDbContext : DbContext
    {
        public WrapGateDbContext(DbContextOptions<WrapGateDbContext> options) : base(options)
        {
        }

        public DbSet<WrapApp> WrapApps { get; set; }
    }
}

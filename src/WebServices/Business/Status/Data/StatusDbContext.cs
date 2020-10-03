using Aiursoft.Status.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Status.Data
{
    public class StatusDbContext : DbContext
    {
        public StatusDbContext(DbContextOptions<StatusDbContext> options) : base(options)
        {
        }

        public DbSet<MonitorRule> MonitorRules { get; set; }
    }
}

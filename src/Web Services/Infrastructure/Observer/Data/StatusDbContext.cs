using Aiursoft.Observer.Models;
using Aiursoft.Observer.SDK.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Aiursoft.Observer.Data
{
    public class ObserverDbContext : DbContext
    {
        public ObserverDbContext(DbContextOptions<ObserverDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<MonitorRule> MonitorRules { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<StatusApp> StatusApps { get; set; }

        public void Seed()
        {
            MonitorRules.RemoveRange(MonitorRules);
            SaveChanges();
            var existingData = MonitorRules.ToList();
            foreach (var item in SeedData.GetRules())
            {
                if (!existingData.Exists(t => t.ProjectName == item.ProjectName))
                {
                    MonitorRules.Add(item);
                }
            }
            SaveChanges();
        }
    }
}

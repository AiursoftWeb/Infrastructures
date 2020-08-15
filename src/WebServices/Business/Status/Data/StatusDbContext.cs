using Aiursoft.Archon.SDK.Services;
using Aiursoft.Observer.SDK.Services;
using Aiursoft.SDK.Services;
using Aiursoft.Status.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Aiursoft.Status.Data
{
    public class StatusDbContext : DbContext
    {
        public StatusDbContext(DbContextOptions<StatusDbContext> options) : base(options)
        {
        }

        public DbSet<MonitorRule> MonitorRules { get; set; }

        public void Seed(IServiceProvider services)
        {
            MonitorRules.RemoveRange(MonitorRules);
            SaveChanges();
            var existingData = MonitorRules.ToList();
            var serviceLocation = services.GetRequiredService<ServiceLocation>();
            var observerLocator = services.GetRequiredService<ObserverLocator>();
            var archonLocator = services.GetRequiredService<ArchonLocator>();
            foreach (var item in SeedData.GetRules(serviceLocation, observerLocator, archonLocator))
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

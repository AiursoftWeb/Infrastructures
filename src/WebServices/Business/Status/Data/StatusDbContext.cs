using Aiursoft.Archon.SDK.Services;
using Aiursoft.DBTools;
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
    }
}

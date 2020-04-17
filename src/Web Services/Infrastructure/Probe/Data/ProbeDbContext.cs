using Aiursoft.Probe.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Probe.Data
{
    public class ProbeDbContext : DbContext
    {
        public ProbeDbContext(DbContextOptions<ProbeDbContext> options) : base(options)
        {
        }

        public DbSet<ProbeApp> Apps { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<File> Files { get; set; }
    }
}

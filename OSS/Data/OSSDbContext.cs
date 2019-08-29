using Aiursoft.Pylon.Models.OSS;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.OSS.Data
{
    public class OSSDbContext : DbContext
    {
        public DbSet<Bucket> Bucket { get; set; }
        public DbSet<OSSFile> OSSFile { get; set; }
        public DbSet<OSSApp> Apps { get; set; }
        public DbSet<Secret> Secrets { get; set; }
        public OSSDbContext(DbContextOptions<OSSDbContext> options) : base(options)
        {

        }
    }
}

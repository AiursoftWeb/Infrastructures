using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.OSS.Models;
using Aiursoft.Pylon.Models.OSS;

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

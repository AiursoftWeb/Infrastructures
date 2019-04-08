using System;
using System.Collections.Generic;
using System.Text;
using Aiursoft.Probe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        public void Seed(IServiceProvider services)
        {

        }
    }
}

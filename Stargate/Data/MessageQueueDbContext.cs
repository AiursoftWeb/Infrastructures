using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Stargate.Models;
using Aiursoft.Pylon.Models.Stargate;

namespace Aiursoft.Stargate.Data
{
    public class StargateDbContext : DbContext
    {
        public StargateDbContext(DbContextOptions<StargateDbContext> options) : base(options)
        {
        }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<StargateApp> Apps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

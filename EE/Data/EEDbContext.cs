using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.EE.Models;

namespace Aiursoft.EE.Data
{
    public class EEDbContext : IdentityDbContext<EEUser>
    {
        public EEDbContext(DbContextOptions<EEDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Follow> Follows { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Colossus.Models;

namespace Aiursoft.Colossus.Data
{
    public class ColossusDbContext : IdentityDbContext<ColossusUser>
    {
        public ColossusDbContext(DbContextOptions<ColossusDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<UploadRecord> UploadRecords { get; set; }
    }
}

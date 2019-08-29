using Aiursoft.Colossus.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

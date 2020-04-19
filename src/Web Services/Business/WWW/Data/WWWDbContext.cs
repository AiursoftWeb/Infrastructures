using Aiursoft.WWW.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.WWW.Data
{
    public class WWWDbContext : IdentityDbContext<WWWUser>
    {
        public WWWDbContext(DbContextOptions<WWWDbContext> options) : base(options)
        {
        }

        public DbSet<SearchHistory> SearchHistories { get; set; }
    }
}

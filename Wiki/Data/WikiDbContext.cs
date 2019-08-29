using Aiursoft.Wiki.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Wiki.Data
{
    public class WikiDbContext : IdentityDbContext<WikiUser>
    {
        public WikiDbContext(DbContextOptions<WikiDbContext> options)
            : base(options)
        {
        }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<Comment> Comment { get; set; }
    }
}

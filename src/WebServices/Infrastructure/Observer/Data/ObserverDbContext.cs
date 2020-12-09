using Aiursoft.Observer.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Observer.Data
{
    public class ObserverDbContext : DbContext
    {
        public ObserverDbContext(DbContextOptions<ObserverDbContext> options) : base(options)
        {
        }

        public DbSet<ErrorLog> ErrorLogs { get; set; }
    }
}

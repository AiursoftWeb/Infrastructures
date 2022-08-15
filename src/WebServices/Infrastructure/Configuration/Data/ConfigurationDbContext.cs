using Aiursoft.Observer.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Configuration.Data
{
    public class ConfigurationDbContext : DbContext
    {
        public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : base(options)
        {
        }

    }
}

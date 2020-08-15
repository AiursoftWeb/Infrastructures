using Aiursoft.Wrap.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Wrap.Data
{
    public class WrapDbContext : IdentityDbContext<WrapUser>
    {
        public WrapDbContext(DbContextOptions<WrapDbContext> options) : base(options)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.WWW.Models;

namespace Aiursoft.WWW.Data
{
    public class WWWDbContext : IdentityDbContext<WWWUser>
    {
        public WWWDbContext(DbContextOptions<WWWDbContext> options) : base(options)
        {
        }
    }
}

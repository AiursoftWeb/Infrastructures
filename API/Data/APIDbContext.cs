using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.API.Models;
using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon;
using Microsoft.Extensions.Logging;
using Aiursoft.Pylon.Services;

namespace Aiursoft.API.Data
{
    public class APIDbContext : IdentityDbContext<APIUser>
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
        }

        public DbSet<OAuthPack> OAuthPack { get; set; }
        public DbSet<AppGrant> LocalAppGrant { get; set; }
        public DbSet<UserEmail> UserEmails { get; set; }
    }
}

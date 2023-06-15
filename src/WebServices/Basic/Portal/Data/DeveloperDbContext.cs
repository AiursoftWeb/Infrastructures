using Aiursoft.Portal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Portal.Data;

public class PortalDbContext : IdentityDbContext<PortalUser>
{
    public PortalDbContext(DbContextOptions<PortalDbContext> options)
        : base(options)
    {
    }
}
using Aiursoft.Account.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Account.Data
{
    public class AccountDbContext : IdentityDbContext<AccountUser>
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options)
            : base(options)
        {

        }
    }
}

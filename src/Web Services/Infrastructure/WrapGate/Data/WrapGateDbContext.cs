using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.WrapGate.Data
{
    public class WrapGateDbContext : DbContext
    {
        public WrapGate(DbContextOptions<WrapGate> options): base(options)
        {
        }
    }
}

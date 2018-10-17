using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public static class ExtendMethods
    {
        public static void Delete<T>(this DbSet<T> dbSet, Func<T, bool> predicate) where T : class
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }
    }
}

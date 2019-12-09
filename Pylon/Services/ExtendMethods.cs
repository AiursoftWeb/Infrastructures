using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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

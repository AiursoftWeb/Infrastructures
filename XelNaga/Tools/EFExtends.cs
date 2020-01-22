using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Aiursoft.XelNaga.Tools
{
    public static class EFExtends
    {
        public static void Delete<T>(this DbSet<T> dbSet, Func<T, bool> predicate) where T : class
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }
    }
}

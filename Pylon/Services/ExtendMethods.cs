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
        public static bool Exists<T>(this IQueryable<T> dbSet, Func<T, bool> predicate) where T : class
        {
            var results = dbSet.Where(predicate);
            return results.Count() > 0;
        }

        public static void Delete<T>(this DbSet<T> dbSet, Func<T, bool> predicate) where T : class
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }

        public static IEnumerable<P> SwitchMap<T,P>(this IEnumerable<T> dbSet, Func<T,P> predicate) where T : class
        {
            List<P> list = new List<P>();
            foreach(var element in dbSet)
            {
                list.Add(predicate(element));
            }
            return list;
        }
    }
}

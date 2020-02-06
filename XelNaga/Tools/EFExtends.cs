using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tools
{
    public static class EFExtends
    {
        public static void Delete<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> predicate) where T : class
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }

        public static async Task Sync<T>(this DbSet<T> dbSet,
            Expression<Func<T, bool>> filter,
            T[] collection) where T : class
        {
            foreach (var item in collection)
            {
                var itemCountShallBe = collection.Count(t => t.Equals(item));
                var itemQuery = dbSet
                    .Where(filter)
                    .Where(t => t.Equals(item));
                var itemCount = await itemQuery
                    .CountAsync();

                if (itemCount > itemCountShallBe)
                {
                    dbSet.RemoveRange(itemQuery.Skip(itemCountShallBe));
                }
                else if (itemCount < itemCountShallBe)
                {
                    for (int i = 0; i < itemCountShallBe - itemCount; i++)
                    {
                        dbSet.Add(item);
                    }
                }
            }

            dbSet.Delete(t => collection.Any(p => p.Equals(t)));
        }
    }
}

// DB: 1 1
// CD: 1 1 2 2 3 3 

// DB: 1 1 1
// CD: 1 1 2

// DB: 1 1 2
// CD: 1 1 1

// DB: 1 1
// CD: 1 1 2 2 3 3
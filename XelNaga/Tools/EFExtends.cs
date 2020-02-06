using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Aiursoft.XelNaga.Tools
{
    public interface ISyncable
    {
        bool EqualsInDb(ISyncable obj);
    }
    public static class EFExtends
    {
        public static IEnumerable<T> DistinctBySync<T>(this IEnumerable<T> query) where T : ISyncable
        {
            var knownKeys = new HashSet<T>();
            foreach (T element in query)
            {
                if (!knownKeys.Any(k => k.EqualsInDb(element)))
                {
                    knownKeys.Add(element);
                    yield return element;
                }
            }
        }

        public static void Delete<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> predicate) where T : class
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }

        public static void Sync<T>(this DbSet<T> dbSet,
            T[] collection) where T : class, ISyncable
        {
            dbSet.Sync(t => true, collection);
        }

        public static void Sync<T>(this DbSet<T> dbSet,
            Expression<Func<T, bool>> filter,
            T[] collection) where T : class, ISyncable
        {
            foreach (var item in collection.DistinctBySync())
            {
                var itemCountShallBe = collection.Count(t => t.EqualsInDb(item));
                var itemQuery = dbSet
                    .IgnoreQueryFilters()
                    .Where(filter)
                    .AsEnumerable()
                    .Where(t => item.EqualsInDb(t));
                var itemCount = itemQuery
                    .Count();

                if (itemCount > itemCountShallBe)
                {
                    dbSet.RemoveRange(itemQuery.Skip(itemCountShallBe));
                }
                else if (itemCount < itemCountShallBe)
                {
                    int times = 0;
                    foreach (var toAdd in collection.Where(t => t.EqualsInDb(item)))
                    {
                        dbSet.Add(toAdd);
                        if (++times >= itemCountShallBe - itemCount)
                        {
                            break;
                        }
                    }
                }
            }
            var toDelete = dbSet.AsEnumerable().Where(t => !collection.Any(p => p.EqualsInDb(t)));
            dbSet.RemoveRange(toDelete);
        }
    }
}
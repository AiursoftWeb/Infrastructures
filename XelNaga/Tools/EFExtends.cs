using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Aiursoft.XelNaga.Tools
{
    public interface ISyncable<T>
    {
        bool EqualsInDb(T obj);
        T Map();

        public bool EqualsInMemory(ISyncable<T> obj)
        {
            return EqualsInDb(obj.Map());
        }
    }
    public static class EFExtends
    {
        public static IEnumerable<M> DistinctBySync<T, M>(this IEnumerable<M> query) where M : ISyncable<T>
        {
            var knownKeys = new HashSet<M>();
            foreach (M element in query)
            {
                if (!knownKeys.Any(k => k.EqualsInMemory(element)))
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

        public static void Sync<T, M>(this DbSet<T> dbSet,
            M[] collection)
            where T : class
            where M : ISyncable<T>
        {
            dbSet.Sync(t => true, collection);
        }

        public static void Sync<T, M>(this DbSet<T> dbSet,
            Expression<Func<T, bool>> filter,
            M[] collection)
            where T : class
            where M : ISyncable<T>
        {
            foreach (var item in collection.DistinctBySync<T, M>())
            {
                var itemCountShallBe = collection.Count(t => t.EqualsInMemory(item));
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
                    for (int i = 0; i < itemCountShallBe - itemCount; i++)
                    {
                        dbSet.Add(item.Map());
                    }
                }
            }
            var toDelete = dbSet.AsEnumerable().Where(t => !collection.Any(p => p.EqualsInDb(t)));
            dbSet.RemoveRange(toDelete);
        }
    }
}
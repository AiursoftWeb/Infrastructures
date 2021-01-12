using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Interfaces;
using Aiursoft.Handler.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aiursoft.DBTools
{
    public interface ISyncable<T>
    {
        bool EqualsInDb(T obj);
        T Map();
    }

    public static class EFExtends
    {
        private static IEnumerable<M> DistinctBySync<T, M>(this IEnumerable<M> query) where M : ISyncable<T>
        {
            var knownKeys = new HashSet<M>();
            foreach (M element in query)
            {
                if (!knownKeys.Any(k => k.EqualsInDb(element.Map())))
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

        public static async Task EnsureUnique<T, V>(this IQueryable<T> query, Expression<Func<T, V>> predicate, V value)
            where T : class
            where V : class
        {
            var conflict = await query.Select(predicate).AnyAsync(v => v == value);
            if (conflict)
            {
                throw new AiurAPIModelException(ErrorType.NotEnoughResources, $"There is already a record with name: '{value}'. Please try another new name.");
            }
        }

        public static async Task EnsureUniqueString<T>(this IQueryable<T> query, Expression<Func<T, string>> predicate, string value)
            where T : class
        {
            var conflict = await query.Select(predicate).AnyAsync(v => v.ToLower() == value.ToLower());
            if (conflict)
            {
                throw new AiurAPIModelException(ErrorType.NotEnoughResources, $"There is already a record with name: '{value}'. Please try another new name.");
            }
        }

        public static void Sync<T, M>(this DbSet<T> dbSet,
            IList<M> collection)
            where T : class
            where M : ISyncable<T>
        {
            dbSet.Sync(t => true, collection);
        }

        public static void Sync<T, M>(this DbSet<T> dbSet,
            Expression<Func<T, bool>> filter,
            IList<M> collection)
            where T : class
            where M : ISyncable<T>
        {
            foreach (var item in collection.DistinctBySync<T, M>())
            {
                var itemCountShallBe = collection.Count(t => t.EqualsInDb(item.Map()));
                var items = dbSet
                    .IgnoreQueryFilters()
                    .Where(filter)
                    .AsEnumerable()
                    .Where(t => item.EqualsInDb(t))
                    .ToList();
                var itemCount = items
                    .Count();

                if (itemCount > itemCountShallBe)
                {
                    dbSet.RemoveRange(items.Skip(itemCountShallBe));
                }
                else if (itemCount < itemCountShallBe)
                {
                    for (int i = 0; i < itemCountShallBe - itemCount; i++)
                    {
                        dbSet.Add(item.Map());
                    }
                }
            }
            var toDelete = dbSet
                .Where(filter)
                .AsEnumerable()
                .Where(t => !collection.Any(p => p.EqualsInDb(t)));
            dbSet.RemoveRange(toDelete);
        }

        public static IQueryable<T> Page<T>(this IOrderedQueryable<T> query, IPageable pager)
        {
            return query
                .Skip((pager.PageNumber - 1) * pager.PageSize)
                .Take(pager.PageSize);
        }
    }
}

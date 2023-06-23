using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.DBTools;

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
        foreach (var element in query)
        {
            if (knownKeys.Any(k => k.EqualsInDb(element.Map())))
            {
                continue;
            }

            knownKeys.Add(element);
            yield return element;
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
            throw new DbUpdateException($"There is already a record with name: '{value}'. Please try another new name.");
        }
    }

    /// <summary>
    ///     If there already exists one with your condition, will create a new AiurAPIModelException to generate a response.
    ///     Suggest using a lock when using this.
    /// </summary>
    /// <typeparam name="T">Type of elements of the query.</typeparam>
    /// <param name="query">Query.</param>
    /// <param name="predicate">Condition predicate.</param>
    /// <param name="value">Unique value.</param>
    /// <returns>Task.</returns>
    public static async Task<bool> EnsureUniqueString<T>(this IQueryable<T> query, Expression<Func<T, string>> predicate,
        string value)
        where T : class
    {
        var conflict = await query.Select(predicate).AnyAsync(v => v.ToLower() == value.ToLower());
        return conflict;
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
            var itemCount = items.Count;

            if (itemCount > itemCountShallBe)
            {
                dbSet.RemoveRange(items.Skip(itemCountShallBe));
            }
            else if (itemCount < itemCountShallBe)
            {
                for (var i = 0; i < itemCountShallBe - itemCount; i++)
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
}
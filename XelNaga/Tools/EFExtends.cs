using Aiursoft.XelNaga.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Aiursoft.XelNaga.Tools
{
    public static class EFExtends
    {
        /// <summary>
        /// Page the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageNumber">Starts from 1</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> Page<T>(this IOrderedQueryable<T> query, IPageable pager)
        {
            return query
                .Skip((pager.PageNumber - 1) * pager.PageSize)
                .Take(pager.PageSize);
        }

        public static void Delete<T>(this DbSet<T> dbSet, Func<T, bool> predicate) where T : class
        {
            dbSet.RemoveRange(dbSet.Where(predicate));
        }
    }
}

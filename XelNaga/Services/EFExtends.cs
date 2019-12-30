using Aiursoft.XelNaga.Interfaces;
using System.Linq;

namespace Aiursoft.XelNaga.Services
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
    }
}

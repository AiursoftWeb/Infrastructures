using Aiursoft.Handler.Interfaces;
using Aiursoft.Handler.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Handler.Models
{
    public class AiurPagedCollection<T> : AiurCollection<T>
    {
        [Obsolete("This method is only for framework", true)]
        public AiurPagedCollection() : base() { }
        private AiurPagedCollection(List<T> items) : base(items) { }
        public static async Task<AiurPagedCollection<T>> BuildAsync(
            IOrderedQueryable<T> query,
            IPageable pager,
            ErrorType code,
            string message)
        {
            var items = await query.Page(pager).ToListAsync();
            return new AiurPagedCollection<T>(items)
            {
                TotalCount = await query.CountAsync(),
                CurrentPage = pager.PageNumber,
                CurrentPageSize = pager.PageSize,
                Code = code,
                Message = message
            };
        }
        public int TotalCount { get; set; }
        /// <summary>
        /// Starts from 1.
        /// </summary>
        public int CurrentPage { get; set; }

        public int CurrentPageSize { get; set; }
    }
}

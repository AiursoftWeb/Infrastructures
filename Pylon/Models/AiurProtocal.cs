using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models
{
    public enum ErrorType
    {
        Success = 0,
        WrongKey = -1,
        Pending = -2,
        RequireAttention = -3,
        NotFound = -4,
        UnknownError = -5,
        HasDoneAlready = -6,
        NotEnoughResources = -7,
        Unauthorized = -8,
        InvalidInput = -10,
        Timeout = -11
    }

    public class AiurProtocol
    {
        public virtual ErrorType Code { get; set; }
        public virtual string Message { get; set; }
    }

    public class AiurValue<T> : AiurProtocol
    {
        public AiurValue(T value)
        {
            Value = value;
        }
        public T Value { get; set; }
    }

    public class AiurCollection<T> : AiurProtocol
    {
        public AiurCollection(List<T> items)
        {
            Items = items;
        }
        public List<T> Items { get; set; }
    }

    public class AiurPagedCollection<T> : AiurCollection<T>
    {
        public AiurPagedCollection(List<T> items) : base(items) { }
        public static async Task<AiurPagedCollection<T>> Build(
            IOrderedQueryable<T> query,
            int pageNumber,
            int pageSize,
            ErrorType code,
            string message)
        {
            var items = await query.Page(pageNumber, pageSize).ToListAsync();
            return new AiurPagedCollection<T>(items)
            {
                TotalCount = await query.CountAsync(),
                CurrentPage = pageNumber,
                Code = code,
                Message = message
            };
        }
        public int TotalCount { get; set; }
        /// <summary>
        /// Starts from 1.
        /// </summary>
        public int CurrentPage { get; set; }
    }
}

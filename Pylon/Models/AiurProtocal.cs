using System.Collections;
using System.Collections.Generic;

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

    public class AiurCollection<T> : AiurProtocol, IEnumerable<T>
    {
        public AiurCollection(List<T> items)
        {
            Items = items;
        }
        public List<T> Items { get; set; }

        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }

    public class AiurPagedCollection<T> : AiurCollection<T>
    {
        public AiurPagedCollection(List<T> items) : base(items) { }
        public int TotalCount { get; set; }
        /// <summary>
        /// Starts from 0.
        /// </summary>
        public int CurrentPage { get; set; }
    }
}

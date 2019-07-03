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
    public class AiurCollection<T> : AiurProtocol
    {
        public AiurCollection(List<T> items)
        {
            Items = items;
        }
        public List<T> Items { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models
{
    public enum ErrorType : int
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
    public class AiurProtocal
    {
        public virtual ErrorType code { get; set; }
        public virtual string message { get; set; }
    }
    public class AiurValue<T> : AiurProtocal
    {
        public AiurValue(T value)
        {
            this.Value = value;
        }
        public virtual T Value { get; set; }
    }
    public class AiurCollection<T> : AiurProtocal
    {
        public AiurCollection(IEnumerable<T> items)
        {
            this.Items = items;
        }
        public virtual IEnumerable<T> Items { get; set; }
    }
}

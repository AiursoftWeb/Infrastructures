using Aiursoft.Handler.Abstract.Models;
using System;

namespace Aiursoft.Handler.Abstract.Exceptions
{
    /// <summary>
    /// Throw this exception in any methods called from API. This will stop the controller logic.
    /// Use together with `AiurExpHandler` will directly return the message as `AiurProtocol`.
    /// </summary>
    public class AiurAPIModelException : Exception
    {
        public ErrorType Code { get; set; }
        public AiurAPIModelException(ErrorType code, string message) : base(message)
        {
            Code = code;
        }
    }
}

using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Exceptions
{
    /// <summary>
    /// Throw this exception in any methods called from API. This will stop the controller logic.
    /// Use together with `AiurExpHandler` will directly return the message as `AiurProtocal`.
    /// </summary>
    public class AiurAPIModelException : Exception
    {
        public ErrorType Code { get; set; }
        public AiurAPIModelException(ErrorType code, string message) : base(message)
        {
            this.Code = code;
        }
    }
}

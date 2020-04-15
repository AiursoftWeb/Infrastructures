using Aiursoft.Handler.Abstract.Models;
using System;

namespace Aiursoft.Handler.Abstract.Exceptions
{
    /// <summary>
    /// Throw this exception if the json responed by the Aiursoft Server was not with code = 0.
    /// Catch it in your own code or just use `AiurExpHandler`.
    /// </summary>
    public class AiurUnexceptedResponse : Exception
    {
        public AiurProtocol Response { get; set; }
        public ErrorType Code => Response.Code;
        public AiurUnexceptedResponse(AiurProtocol response) : base(response.Message)
        {
            Response = response;
        }
    }
}

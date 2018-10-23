using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Exceptions
{
    /// <summary>
    /// Throw this exception if the json responed by the Aiursoft Server was not with code = 0.
    /// Catch it in your own code or just use `AiurExpHandler`.
    /// </summary>
    public class AiurUnexceptedResponse : Exception
    {
        public AiurProtocal Response { get; set; }
        public ErrorType Code => Response.Code;
        public AiurUnexceptedResponse(AiurProtocal response) : base(response.Message)
        {
            Response = response;
        }
    }
}

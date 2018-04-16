using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Exceptions
{
    public class AiurUnexceptedResponse : Exception
    {
        public AiurProtocal Response { get; set; }
        public AiurUnexceptedResponse(AiurProtocal response)
        {
            Response = response;
        }
        public override string Message => Response.message;
    }
}

using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Exceptions
{
    public class AiurAPIModelException : Exception
    {
        public AiurAPIModelException(ErrorType code, string message) : base(message)
        {
            this.Code = code;
        }
        public ErrorType Code { get; set; }
    }
}

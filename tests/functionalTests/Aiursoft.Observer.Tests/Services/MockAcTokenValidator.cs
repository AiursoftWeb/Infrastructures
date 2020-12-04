using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Observer.Tests.Services
{
    public class MockAcTokenValidator : ACTokenValidator
    {
        public MockAcTokenValidator(RSAService rsa) : base(rsa)
        {
        }

        public override string ValidateAccessToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "Mock token was not in a valid format and can not be verified!");
            }
            return "mock-app-id";
        }
    }
}

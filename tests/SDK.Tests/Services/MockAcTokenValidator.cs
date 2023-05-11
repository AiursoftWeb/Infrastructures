using System;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;

namespace Aiursoft.SDK.Tests.Services;

public class MockAcTokenValidator : ACTokenValidator
{
    public static string MockAppId = Guid.NewGuid().ToString();
    public static string Mock2AppId = Guid.NewGuid().ToString();

    public MockAcTokenValidator(RSAService rsa) : base(rsa)
    {
    }

    public override string ValidateAccessToken(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("mock-"))
        {
            return MockAppId;
        }

        if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("mock2-"))
        {
            return Mock2AppId;
        }

        throw new AiurAPIModelException(ErrorType.Unauthorized,
            "Mock token was not in a valid format and can not be verified!");
    }
}
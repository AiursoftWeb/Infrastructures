using System;
using System.Threading.Tasks;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;

namespace Aiursoft.SDK.Tests.Services;

public class MockAcTokenValidator : AiursoftAppTokenValidator
{
    public static string MockAppId = Guid.NewGuid().ToString();
    public static string Mock2AppId = Guid.NewGuid().ToString();

    public MockAcTokenValidator(GatewayRSAService rsa) : base(rsa)
    {
    }

    public override Task<string> ValidateAccessTokenAsync(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("mock-"))
        {
            return Task.FromResult(MockAppId);
        }

        if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("mock2-"))
        {
            return Task.FromResult(Mock2AppId);
        }

        throw new AiurAPIModelException(ErrorType.Unauthorized,
            "Mock token was not in a valid format and can not be verified!");
    }
}
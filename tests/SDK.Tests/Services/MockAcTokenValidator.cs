using Aiursoft.AiurProtocol;
using Aiursoft.Directory.SDK.Services;

namespace Aiursoft.SDK.Tests.Services;

public class MockAcTokenValidator : AiursoftAppTokenValidator
{
    public static readonly string MockAppId = Guid.NewGuid().ToString();
    public static readonly string Mock2AppId = Guid.NewGuid().ToString();

    public MockAcTokenValidator(AppTokenRsaService rsa) : base(rsa)
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

        throw new AiurServerException(Code.Unauthorized,
            "Mock token was not in a valid format and can not be verified!");
    }
}
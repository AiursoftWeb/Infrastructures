using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Scanner.Abstraction;

namespace Aiursoft.Probe.Services;

public class TokenEnsurer : ITransientDependency
{
    private readonly ProbeTokenManager _probeTokenManager;

    public TokenEnsurer(ProbeTokenManager probeTokenManager)
    {
        _probeTokenManager = probeTokenManager;
    }

    public void Ensure(string pbToken, string action, string siteName, string folderNames)
    {
        var token = _probeTokenManager.ValidateAccessToken(pbToken);
        if (token.SiteName != siteName)
        {
            throw new AiurServerException(Code.Unauthorized,
                $"Your token was not authorized to {action.ToLower()} files to this site: '{siteName}'.");
        }

        if (!token.Permissions.ToLower().Contains(action.ToLower().Trim()))
        {
            throw new AiurServerException(Code.Unauthorized,
                $"Your token was not authorized to {action.ToLower()}. Your token is only permitted to '{token.Permissions}'");
        }

        if (!string.IsNullOrWhiteSpace(token.UnderPath) && folderNames != null &&
            !folderNames.StartsWith(token.UnderPath))
        {
            throw new AiurServerException(Code.Unauthorized,
                $"Your token is only authorized to {action.ToLower()} files from path: '{token.UnderPath}', not '{folderNames}'.");
        }
    }
}
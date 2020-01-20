using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Exceptions;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.Probe.Services
{
    public class TokenEnsurer : ITransientDependency
    {
        private readonly PBTokenManager _pbTokenManager;

        public TokenEnsurer(PBTokenManager pbTokenManager)
        {
            _pbTokenManager = pbTokenManager;
        }

        public void Ensure(string pbToken, string action, string siteName, string folderNames)
        {
            var token = _pbTokenManager.ValidateAccessToken(pbToken);
            if (token.SiteName != siteName)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, $"Your token was not authorized to {action.ToLower()} files to this site: '{siteName}'.");
            }
            if (!token.Permissions.ToLower().Contains(action.ToLower().Trim()))
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, $"Your token was not authorized to {action.ToLower()}. Your token is only permitted to '{token.Permissions}'");
            }
            if (!string.IsNullOrWhiteSpace(token.UnderPath) && folderNames != null && !folderNames.StartsWith(token.UnderPath))
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, $"Your token is only authorized to {action.ToLower()} files from path: '{token.UnderPath}', not '{folderNames}'.");
            }
        }
    }
}

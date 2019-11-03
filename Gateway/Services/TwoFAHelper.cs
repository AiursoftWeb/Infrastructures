using Aiursoft.Gateway.Models;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class TwoFAHelper : ITransientDependency
    {
        private readonly UserManager<GatewayUser> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private const string _authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public TwoFAHelper(
             UserManager<GatewayUser> userManager,
              UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        public async Task<(string, string)> LoadSharedKeyAndQrCodeUriAsync(GatewayUser user)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            var twoFAKey = string.Join(' ', unformattedKey.SplitInParts(4));
            var twoFAQRUri = GenerateQrCodeUri(user.Email, unformattedKey);

            return (twoFAKey, twoFAQRUri);
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                _authenticatorUriFormat,
                _urlEncoder.Encode("Aiursoft.Nexus.Account"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}

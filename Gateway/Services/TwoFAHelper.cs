using Aiursoft.Gateway.Models;
using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models.API.UserAddressModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class TwoFAHelper: IScopedDependency
    {
        private readonly UserManager<GatewayUser> _userManager;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public TwoFAHelper(
             UserManager<GatewayUser> userManager,
              UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        public async Task<List<Get2FAKeyAddressModel>> LoadSharedKeyAndQrCodeUriAsync(GatewayUser user, Get2FAKeyAddressModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            model.TwoFAKey = FormatKey(unformattedKey);
            model.TwoFAQRUri = GenerateQrCodeUri(user.Email, unformattedKey);

            var returnList = new List<Get2FAKeyAddressModel>();
            returnList.Add(model);

            return returnList;
        }

        public string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        public string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Aiursoft.Nexus.Account"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}

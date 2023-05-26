using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Aiursoft.Gateway.Models;
using Aiursoft.Scanner.Abstract;
using Aiursoft.SDK;
using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Directory.Services;

public class TwoFAHelper : ITransientDependency
{
    private const string _authenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    private readonly UrlEncoder _urlEncoder;
    private readonly UserManager<DirectoryUser> _userManager;

    public TwoFAHelper(
        UserManager<DirectoryUser> userManager,
        UrlEncoder urlEncoder)
    {
        _userManager = userManager;
        _urlEncoder = urlEncoder;
    }

    public async Task<(string, string)> LoadSharedKeyAndQrCodeUriAsync(DirectoryUser user)
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
            _urlEncoder.Encode(Values.ProjectName),
            _urlEncoder.Encode(email),
            unformattedKey);
    }
}
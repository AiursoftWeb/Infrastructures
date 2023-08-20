using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.Directory.Models.OAuthViewModels;

public class AuthorizeViewModel : FinishAuthInfo
{
    [Obsolete("This method is only for framework!", true)]
    public AuthorizeViewModel()
    {
    }

    public AuthorizeViewModel(string redirectUri, string state, string appId, string appName, string appImageUrl,
        bool allowRegistering, bool allowPasswordSignIn)
    {
        RedirectUri = redirectUri;
        State = state;
        AppId = appId;
        Recover(appName, appImageUrl, allowRegistering, allowPasswordSignIn);
    }

    // Display part:
    public string AppName { get; set; }
    public string AppImageUrl { get; set; }
    public bool AllowRegistering { get; set; }
    public bool AllowPasswordSignIn { get; set; }

    // Submit part:
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [Display(Name = "Aiursoft Account")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [NoSpace]
    public string Password { get; set; }

    public void Recover(string appName, string appImageUrl, bool allowRegistering, bool allowPasswordSignIn)
    {
        AppName = appName;
        AppImageUrl = appImageUrl;
        AllowRegistering = allowRegistering;
        AllowPasswordSignIn = allowPasswordSignIn;
    }
}
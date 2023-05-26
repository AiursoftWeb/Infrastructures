using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Identity;

namespace Aiursoft.Directory.Models.OAuthViewModels;

public class AuthorizeConfirmViewModel : FinishAuthInfo
{
    // Display part:
    public string AppName { get; set; }
    public string UserNickName { get; set; }
    public string Email { get; set; }

    [Display(Name = "View your basic identity info")]
    public bool ViewOpenId { get; set; } = true;

    [Display(Name = "View your phone number")]
    public bool ViewPhoneNumber { get; set; }

    [Display(Name = "Change your phone number")]
    public bool ChangePhoneNumber { get; set; }

    [Display(Name = "Change your Email confirmation status")]
    public bool ConfirmEmail { get; set; }

    [Display(Name = "Change your basic info like nickname and bio")]
    public bool ChangeBasicInfo { get; set; }

    [Display(Name = "Request to change your password")]
    public bool ChangePassword { get; set; }

    [Display(Name = "Change user's other applications' grant status")]
    public bool ChangeGrantInfo { get; set; }

    [Display(Name = "View your sign in log.")]
    public bool ViewAuditLog { get; set; }

    [Display(Name = "View your bound social accounts.")]
    public bool ManageSocialAccount { get; set; }

    public string TermsUrl { get; set; }
    public string PStatementUrl { get; set; }
    public string FailBackUrl { get; set; }

    public string GetRedirectRoot()
    {
        if (!string.IsNullOrWhiteSpace(FailBackUrl))
        {
            return FailBackUrl;
        }

        var url = new Uri(RedirectUri);
        return $@"{url.Scheme}://{url.Host}/?{AuthValues.DirectShowString.Key}={AuthValues.DirectShowString.Value}";
    }
}
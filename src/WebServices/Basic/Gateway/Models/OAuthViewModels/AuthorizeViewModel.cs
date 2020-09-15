using Aiursoft.Gateway.SDK.Models;
using Aiursoft.SDKTools.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.OAuthViewModels
{
    public class AuthorizeViewModel : FinishAuthInfo
    {
        [Obsolete("This method is only for framework!", true)]
        public AuthorizeViewModel() { }
        public AuthorizeViewModel(string redirectUri, string state, string appId, string appName, string appImageUrl, bool allowRegistering)
        {
            RedirectUri = redirectUri;
            State = state;
            AppId = appId;
            Recover(appName, appImageUrl, allowRegistering);
        }

        public void Recover(string appName, string appImageUrl, bool allowRegistering)
        {
            AppName = appName;
            AppImageUrl = appImageUrl;
            AllowRegistering = allowRegistering;
        }

        // Display part:
        public string AppName { get; set; }
        public string AppImageUrl { get; set; }
        public bool AllowRegistering { get; set; }

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
    }
}

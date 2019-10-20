using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.OAuthViewModels
{
    public class AuthorizeViewModel : FinishAuthInfo
    {
        [Obsolete("This method is only for framework!", true)]
        public AuthorizeViewModel() { }
        public AuthorizeViewModel(string redirectUrl, string state, string appId, string appName, string appImageUrl)
        {
            RedirectUrl = redirectUrl;
            State = state;
            AppId = appId;
            Recover(appName, appImageUrl);
        }

        public void Recover(string appName, string appImageUrl)
        {
            AppName = appName;
            AppImageUrl = appImageUrl;
        }

        // Display part:
        public string AppName { get; set; }
        public string AppImageUrl { get; set; }

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

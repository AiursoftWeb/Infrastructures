using Aiursoft.SDK.Models;
using Aiursoft.WebTools.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.OAuthViewModels
{
    public class RegisterViewModel : FinishAuthInfo
    {
        [Obsolete(error: true, message: "This function is only for framework.")]
        public RegisterViewModel() { }
        public RegisterViewModel(string redirectUri, string state, string appId, string appName, string appImageUrl)
        {
            RedirectUri = redirectUri;
            State = state;
            AppId = appId;
            Recover(appName, appImageUrl);
        }
        public void Recover(string appName, string appImageUrl)
        {
            AppName = appName;
            AppImageUrl = appImageUrl;
        }

        // Display part
        public string AppName { get; set; }
        public string AppImageUrl { get; set; }

        // Submit part
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [NoSpace]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Captcha")]
        [Required]
        [StringLength(4)]
        public string CaptchaCode { get; set; }

        public string PreferedLanguage { get; set; }
    }
}

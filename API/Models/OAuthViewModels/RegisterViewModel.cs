using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.API.Models.OAuthViewModels
{
    public class RegisterViewModel : IAuthorizeViewModel
    {
        [Obsolete(error: true, message: "This function is only for framework.")]
        public RegisterViewModel() { }
        public RegisterViewModel(string toRedirect, string state, string appId, string scope, string responseTpe, string appName, string appImageUrl)
        {
            this.ToRedirect = toRedirect;
            this.State = state;
            this.AppId = appId;
            this.Scope = scope;
            this.ResponseType = responseTpe;
            Recover(appName, appImageUrl);
        }
        public void Recover(string appName, string appImageUrl)
        {
            AppName = appName;
            AppImageUrl = appImageUrl;
        }

        [Url]
        public string ToRedirect { get; set; }
        public string State { get; set; }
        public string AppId { get; set; }
        public string ResponseType { get; set; }
        public string Scope { get; set; }

        public string AppName { get; set; }
        public string AppImageUrl { get; set; }

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

        public string GetRegexRedirectUrl()
        {
            var url = new Uri(ToRedirect);
            string result = $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
            return result;
        }
    }
}

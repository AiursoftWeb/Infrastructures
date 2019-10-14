using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.OAuthViewModels
{
    public class AuthorizeViewModel : IAuthorizeViewModel
    {
        [Obsolete("This method is only for framework!", true)]
        public AuthorizeViewModel() { }
        public AuthorizeViewModel(string toRedirect, string state, string appId, string scope, string responseType, string appName, string appImageUrl)
        {
            this.ToRedirect = toRedirect;
            this.State = state;
            this.AppId = appId;
            this.Scope = scope;
            this.ResponseType = responseType;
            Recover(appName, appImageUrl);
        }
        public void Recover(string appName, string appImageUrl)
        {
            this.AppName = appName;
            this.AppImageUrl = appImageUrl;
        }
        public string AppImageUrl { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Aiursoft Account")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [NoSpace]
        public string Password { get; set; }

        [Required]
        [Url]
        public string ToRedirect { get; set; }

        public string State { get; set; }
        [Required]
        public string AppId { get; set; }
        public string ResponseType { get; set; }
        public string Scope { get; set; }
        public string AppName { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public bool RequiresTwoFactor { get; set; }

        public string GetRegexRedirectUrl()
        {
            var url = new Uri(ToRedirect);
            string result = $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
            return result;
        }
    }
}

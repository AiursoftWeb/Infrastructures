using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.API.Models.OAuthViewModels
{

    public class RegisterViewModel : IAuthorizeViewModel
    {
        [Obsolete(error: true, message: "This function is only for framework.")]
        public RegisterViewModel() { }
        public RegisterViewModel(string ToRedirect, string State, string AppId, string Scope, string ResponseTpe, string AppName, string AppImageUrl)
        {
            this.ToRedirect = ToRedirect;
            this.State = State;
            this.AppId = AppId;
            this.Scope = Scope;
            this.ResponseType = ResponseTpe;
            Recover(AppName, AppImageUrl);
        }
        public void Recover(string AppName, string AppImageUrl)
        {
            this.AppName = AppName;
            this.AppImageUrl = AppImageUrl;
        }

        [Url]
        public virtual string ToRedirect { get; set; }
        public virtual string State { get; set; }
        public virtual string AppId { get; set; }
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

        public string PreferedLanguage { get; set; }

        public string GetRegexRedirectUrl()
        {
            var url = new Uri(ToRedirect);
            string result = $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
            return result;
        }
    }
}

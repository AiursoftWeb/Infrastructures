using Aiursoft.Pylon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.API.Models.OAuthViewModels
{
    public class AuthorizeViewModel : IAuthorizeViewModel
    {
        [Obsolete("This method is only for framework!", true)]
        public AuthorizeViewModel() { }
        public AuthorizeViewModel(string ToRedirect, string State, string AppId, string Scope, string ResponseTpe, string AppName, string AppImageUrl)
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
        public string AppImageUrl { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Aiursoft Account")]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
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
        public string GetRegexRedirectUrl()
        {
            var url = new Uri(ToRedirect);
            string result = $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
            return result;
        }
    }
}

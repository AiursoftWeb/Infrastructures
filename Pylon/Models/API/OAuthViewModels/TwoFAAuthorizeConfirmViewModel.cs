using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.OAuthViewModels
{
    public class TwoFAAuthorizeConfirmViewModel : IAuthorizeViewModel
    {
        public virtual string AppName { get; set; }
        public virtual string UserNickName { get; set; }

        [Url]
        [Required]
        public string ToRedirect { get; set; }

        public string State { get; set; }
        public string AppId { get; set; }
        public string Scope { get; set; }
        public string ResponseType { get; set; }
        public string Email { get; set; }

        [Display(Name = "Verify Code")]
        public string VerifyCode { get; set; }

        public string TermsUrl { get; set; }
        public string PStatementUrl { get; set; }

        public string GetRegexRedirectUrl()
        {
            var url = new Uri(ToRedirect);
            string result = $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
            return result;
        }

        public string GetRedirectRoot()
        {
            var url = new Uri(ToRedirect);
            return $@"{url.Scheme}://{url.Host}/?{Values.DirectShowString.Key}={Values.DirectShowString.Value}";
        }
    }
}

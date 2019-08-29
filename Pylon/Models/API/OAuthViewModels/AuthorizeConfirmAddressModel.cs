using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.OAuthViewModels
{
    public class AuthorizeConfirmAddressModel
    {
        [Required]
        public virtual string AppId { get; set; }

        public string State { get; set; }
        [Required]
        public string ToRedirect { get; set; }
        public string Scope { get; set; }
        public string ResponseType { get; set; }

        public string GetRegexRedirectUrl()
        {
            var url = new Uri(ToRedirect);
            string result = $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
            return result;
        }
    }
}

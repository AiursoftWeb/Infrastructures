using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.OAuthAddressModels
{
    public class AuthorizeAddressModel
    {
        [Required]
        public string appid { get; set; }
        [Required]
        [Url]
        public string redirect_uri { get; set; }
        public string state { get; set; }
        public string scope { get; set; }
        public string response_type { get; set; }
        /// <summary>
        /// Force the user to input his password even when he is already signed in.
        /// </summary>
        public bool? forceConfirm { get; set; } = null;
        /// <summary>
        /// If the user is not signed in, just redirect back not let him input his info.
        /// </summary>
        public bool? tryAutho { get; set; } = null;
        public string GetRegexRedirectUrl()
        {
            var url = new Uri(redirect_uri);
            string result = $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
            return result;
        }
        public IAuthorizeViewModel Convert(string email)
        {
            return new OAuthInfo
            {
                AppId = appid,
                Email = email,
                State = state,
                ToRedirect = redirect_uri,
                Scope = scope,
                ResponseType = response_type
            };
        }
    }
}

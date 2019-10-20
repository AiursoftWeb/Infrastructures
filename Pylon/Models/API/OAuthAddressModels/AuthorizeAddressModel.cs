using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.OAuthAddressModels
{
    public class AuthorizeAddressModel
    {
        [Required]
        [FromQuery(Name = "appid")]
        public string AppId { get; set; }
        [Url]
        [Required]
        [FromQuery(Name = "redirect_uri")]
        public string RedirectUrl { get; set; }
        [FromQuery(Name = "state")]
        public string State { get; set; }
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

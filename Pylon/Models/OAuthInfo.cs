using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models
{
    public class OAuthInfo : IAuthorizeViewModel
    {
        public string State { get; set; }
        public string ToRedirect { get; set; }
        public string Email { get; set; }
        public string AppId { get; set; }
        public string Scope { get; set; }
        public string ResponseType { get; set; }
        public virtual string GetRegexRedirectUrl()
        {
            var url = new Uri(ToRedirect);
            string result = $@"{url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}";
            return result;
        }
    }
}

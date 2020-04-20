using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.SDK.Services.Authentication.ToGitHubServer
{
    public class GitHubAuthAddressModel
    {
        [FromQuery(Name = "client_id")]
        public string ClientId { get; set; }
        [FromQuery(Name = "redirect_uri")]
        public string RedirectUri { get; set; }
        [FromQuery(Name = "state")]
        public string State { get; set; }
    }
}

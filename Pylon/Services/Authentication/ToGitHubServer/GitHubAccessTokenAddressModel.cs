using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Services.Authentication.ToGitHubServer
{
    public class GitHubAccessTokenAddressModel
    {
        [FromQuery(Name = "client_id")]
        public string ClientId { get; set; }
        [FromQuery(Name = "client_secret")]
        public string ClientSecret { get; set; }
        [FromQuery(Name = "code")]
        public string Code { get; set; }
    }
}

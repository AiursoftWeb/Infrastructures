using Newtonsoft.Json;

namespace Aiursoft.Pylon.Services.Authentication.ToGitHubServer
{
    public class GitHubUserDetail : IUserDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonProperty("login")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("bio")]
        public string Bio { get; set; }
    }
}

using Newtonsoft.Json;

namespace Aiursoft.Pylon.Services.Authentication.ToGitHubServer
{
    public class GitHubUserDetail : IUserDetail
    {
        public int Id { get; set; }
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
    }
}

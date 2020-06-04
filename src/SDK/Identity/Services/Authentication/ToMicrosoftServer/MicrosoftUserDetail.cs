using Newtonsoft.Json;

namespace Aiursoft.Identity.Services.Authentication.ToMicrosoftServer
{
    public class MicrosoftUserDetail : IUserDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonProperty("displayName")]
        public string Name { get; set; }
        [JsonProperty("userPrincipalName")]
        public string Email { get; set; }
        [JsonProperty("jobTitle")]
        public string Bio { get; set; }
    }
}

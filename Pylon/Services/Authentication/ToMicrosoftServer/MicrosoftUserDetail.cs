using Aiursoft.XelNaga.Services.Authentication;
using Newtonsoft.Json;

namespace Aiursoft.Pylon.Services.Authentication.ToMicrosoftServer
{
    public class MicrosoftUserDetail : IUserDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonProperty("displayName")]
        public string Name { get; set; }
        [JsonProperty("mail")]
        public string Email { get; set; }
        [JsonProperty("jobTitle")]
        public string Bio { get; set; }
    }
}

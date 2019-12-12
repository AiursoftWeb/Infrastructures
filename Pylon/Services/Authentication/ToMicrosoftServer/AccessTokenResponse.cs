using Newtonsoft.Json;

namespace Aiursoft.Pylon.Services.Authentication.ToMicrosoftServer
{
    public class AccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}

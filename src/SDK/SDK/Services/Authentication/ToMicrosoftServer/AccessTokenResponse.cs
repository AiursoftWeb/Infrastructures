using Newtonsoft.Json;

namespace Aiursoft.SDK.Services.Authentication.ToMicrosoftServer
{
    public class AccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}

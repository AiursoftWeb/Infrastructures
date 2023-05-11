using Newtonsoft.Json;

namespace Aiursoft.Identity.Services.Authentication.ToFaceBookServer;

public class AccessTokenResponse
{
    [JsonProperty("access_token")] public string AccessToken { get; set; }
}
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Services.Authentication.ToMicrosoftServer
{
    //https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?
    //client_id=6731de76-14a6-49ae-97bc-6eba6914391e
    //&response_type=id_token
    //&redirect_uri=http%3A%2F%2Flocalhost%2Fmyapp%2F
    //&scope=openid
    //&response_mode=fragment
    //&state=12345
    //&nonce=678910

    public class MicrosoftAuthAddressModel
    {
        [FromQuery(Name = "client_id")]
        public string ClientId { get; set; }
        [FromQuery(Name = "response_type")]
        public string ResponseType { get; set; }
        [FromQuery(Name = "redirect_uri")]
        public string RedirectUri { get; set; }
        [FromQuery(Name = "scope")]
        public string Scope { get; set; }
        [FromQuery(Name = "response_mode")]
        public string ResponseMode { get; set; }
        [FromQuery(Name = "state")]
        public string State { get; set; }
        [FromQuery(Name = "nonce")]
        public string Nonce { get; set; }
    }
}

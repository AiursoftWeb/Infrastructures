using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Services.Authentication.ToMicrosoftServer
{
    public class MicrosoftAccessTokenAddressModel
    {
        [FromQuery(Name = "client_id")]
        public string ClientId { get; set; }
        [FromQuery(Name = "client_secret")]
        public string ClientSecret { get; set; }
        [FromQuery(Name = "response_type")]
        public string ResponseType { get; set; }
        [FromQuery(Name = "redirect_uri")]
        public string RedirectUri { get;set; }
        [FromQuery(Name = "response_mode")]
        public string ResponseMode { get; set; }
        [FromQuery(Name = "scope")]
        public string Scope { get; set; }
        [FromQuery(Name = "state")]
        public string State { get; set; }
    }
}

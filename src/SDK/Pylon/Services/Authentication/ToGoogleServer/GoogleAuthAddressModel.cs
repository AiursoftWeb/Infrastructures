using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Services.Authentication.ToGoogerServer
{
    public class GoogleAuthAddressModel
    {
        [FromQuery(Name = "client_id")]
        public string ClientId { get; set; }
        [FromQuery(Name = "redirect_uri")]
        public string RedirectUri { get; set; }
        [FromQuery(Name = "state")]
        public string State { get; set; }
        [FromQuery(Name = "scope")]
        public string Scope { get; set; }
        [FromQuery(Name = "response_type")]
        public string ResponseType { get; set; }
    }
}

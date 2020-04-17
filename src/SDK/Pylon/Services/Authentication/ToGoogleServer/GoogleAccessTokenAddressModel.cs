using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Services.Authentication.ToGoogerServer
{
    public class GoogleAccessTokenAddressModel
    {
        [FromQuery(Name = "client_id")]
        public string ClientId { get; set; }
        [FromQuery(Name = "client_secret")]
        public string ClientSecret { get; set; }
        [FromQuery(Name = "code")]
        public string Code { get; set; }
        [FromQuery(Name = "grant_type")]
        public string GrantType { get; set; }
        [FromQuery(Name = "redirect_uri")]
        public string RedirectUri { get; internal set; }
    }
}

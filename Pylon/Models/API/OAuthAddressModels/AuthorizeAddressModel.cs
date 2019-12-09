using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Models.API.OAuthAddressModels
{
    public class AuthorizeAddressModel : FinishAuthInfo
    {
        [FromQuery(Name = "force-confirm")]
        public bool? ForceConfirm { get; set; } = null;

        [FromQuery(Name = "try-auth")]
        public bool? TryAutho { get; set; } = null;
    }
}

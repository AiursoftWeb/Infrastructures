using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Gateway.SDK.Models.API.OAuthAddressModels;

public class AuthorizeAddressModel : FinishAuthInfo
{
    [FromQuery(Name = "force-confirm")] public bool? ForceConfirm { get; set; }

    [FromQuery(Name = "try-auth")] public bool? TryAutho { get; set; }
}
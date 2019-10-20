using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.OAuthAddressModels
{
    public class AuthorizeAddressModel : FinishAuthInfo
    {
        [FromQuery(Name = "force-confirm")]
        public bool? ForceConfirm { get; set; } = null;

        [FromQuery(Name = "try-auth")]
        public bool? tryAutho { get; set; } = null;
    }
}

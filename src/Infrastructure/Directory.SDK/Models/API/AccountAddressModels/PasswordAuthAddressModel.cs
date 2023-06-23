using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Directory.SDK.Models.API.AccountAddressModels;

public class PasswordAuthAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    [EmailAddress] [Required] public string Email { get; set; }

    [Required] [NoSpace] public string Password { get; set; }
}
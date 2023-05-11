using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Gateway.SDK.Models.API.AccountAddressModels;

public class PasswordAuthAddressModel
{
    [Required] public string AccessToken { get; set; }

    [EmailAddress] [Required] public string Email { get; set; }

    [Required] [NoSpace] public string Password { get; set; }
}
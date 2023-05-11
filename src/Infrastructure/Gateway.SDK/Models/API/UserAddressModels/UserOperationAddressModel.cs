using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.SDK.Models.API.UserAddressModels;

public class UserOperationAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required] public string OpenId { get; set; }
}
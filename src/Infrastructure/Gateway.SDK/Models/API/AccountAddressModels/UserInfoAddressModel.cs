using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.SDK.Models.API.AccountAddressModels;

public class UserInfoAddressModel
{
    [Required] public virtual string AccessToken { get; set; }

    [Required] public virtual string OpenId { get; set; }
}
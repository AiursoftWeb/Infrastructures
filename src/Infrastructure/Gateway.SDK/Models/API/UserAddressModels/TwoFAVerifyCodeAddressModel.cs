using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.SDK.Models.API.UserAddressModels;

public class TwoFAVerifyCodeAddressModel : UserOperationAddressModel
{
    [Required] [StringLength(6)] public string Code { get; set; }
}
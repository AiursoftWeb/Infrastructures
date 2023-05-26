using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.SDK.Models.API.UserAddressModels;

public class SetPrimaryEmailAddressModel : UserOperationAddressModel
{
    [EmailAddress] [Required] public string Email { get; set; }
}
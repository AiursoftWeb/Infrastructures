using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.SDK.Models.API.UserAddressModels;

public class BindNewEmailAddressModel : UserOperationAddressModel
{
    [Required]
    [MaxLength(30)]
    [EmailAddress]
    public string NewEmail { get; set; }
}
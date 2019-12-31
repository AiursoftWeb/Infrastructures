using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.API.UserAddressModels
{
    public class SendConfirmationEmailAddressModel : UserOperationAddressModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class TwoFAVerificyCodeAddressModel : UserOperationAddressModel
    {
        [Required]
        public string Code { get; set; }
    }
}

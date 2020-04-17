using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.SDK.Models.API.UserAddressModels
{
    public class TwoFAVerificyCodeAddressModel : UserOperationAddressModel
    {
        [Required]
        [StringLength(6)]
        public string Code { get; set; }
    }
}

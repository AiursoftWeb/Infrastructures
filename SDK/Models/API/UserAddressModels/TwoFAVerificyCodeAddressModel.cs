using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.API.UserAddressModels
{
    public class TwoFAVerificyCodeAddressModel : UserOperationAddressModel
    {
        [Required]
        [StringLength(6)]
        public string Code { get; set; }
    }
}

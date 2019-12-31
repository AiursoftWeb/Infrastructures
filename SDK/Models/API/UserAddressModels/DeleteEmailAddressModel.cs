using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.API.UserAddressModels
{
    public class DeleteEmailAddressModel : UserOperationAddressModel
    {
        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string ThatEmail { get; set; }
    }
}

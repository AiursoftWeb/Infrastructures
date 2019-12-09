using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class UserOperationAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string OpenId { get; set; }
    }
}

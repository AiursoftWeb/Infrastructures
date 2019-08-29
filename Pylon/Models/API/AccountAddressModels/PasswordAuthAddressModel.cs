using Aiursoft.Pylon.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.AccountAddressModels
{
    public class PasswordAuthAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        [NoSpace]
        public string Password { get; set; }
    }
}

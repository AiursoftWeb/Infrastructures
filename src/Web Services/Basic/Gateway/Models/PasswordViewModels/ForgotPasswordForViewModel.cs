using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.PasswordViewModels
{
    public class ForgotPasswordForViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

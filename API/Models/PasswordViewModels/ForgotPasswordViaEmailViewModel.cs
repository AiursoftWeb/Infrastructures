using System.ComponentModel.DataAnnotations;

namespace Aiursoft.API.Models.PasswordViewModels
{
    public class ForgotPasswordViaEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

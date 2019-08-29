using System.ComponentModel.DataAnnotations;

namespace Aiursoft.API.Models.PasswordViewModels
{
    public class ForgotPasswordForViewModel
    {
        public bool ModelStateValid { get; set; } = true;
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

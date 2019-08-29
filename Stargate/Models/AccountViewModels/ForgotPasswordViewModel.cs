using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Stargate.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

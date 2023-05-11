using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.PasswordViewModels;

public class ForgotPasswordViaEmailViewModel
{
    [Required] [EmailAddress] public string Email { get; set; }
}
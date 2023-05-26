using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.Models.PasswordViewModels;

public class ForgotPasswordViaEmailViewModel
{
    [Required] [EmailAddress] public string Email { get; set; }
}
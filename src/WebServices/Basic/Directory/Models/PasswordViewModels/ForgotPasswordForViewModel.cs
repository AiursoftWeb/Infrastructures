using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.Models.PasswordViewModels;

public class ForgotPasswordForViewModel
{
    [Required] [EmailAddress] public string Email { get; set; }
}
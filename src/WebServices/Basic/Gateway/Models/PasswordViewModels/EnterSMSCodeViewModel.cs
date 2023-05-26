using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.Models.PasswordViewModels;

public class EnterSMSCodeViewModel
{
    public string PhoneLast { get; set; }

    [Required] public string Code { get; set; }

    [Required] public string Email { get; set; }
}
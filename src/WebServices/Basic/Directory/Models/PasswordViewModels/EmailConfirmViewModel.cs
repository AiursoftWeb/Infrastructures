using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Directory.Models.PasswordViewModels;

public class EmailConfirmViewModel
{
    public string UserId { get; set; }
    public string Code { get; set; }

    [Display(Name = "Your Email address to confirm")]
    public string Mail { get; internal set; }
}
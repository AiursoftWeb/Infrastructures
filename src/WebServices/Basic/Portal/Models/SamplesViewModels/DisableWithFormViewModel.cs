using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Portal.Models.SamplesViewModels;

public class DisableWithFormViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Any Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Any Password")]
    public string Password { get; set; }
}
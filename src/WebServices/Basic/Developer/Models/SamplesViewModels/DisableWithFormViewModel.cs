using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.SamplesViewModels;

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
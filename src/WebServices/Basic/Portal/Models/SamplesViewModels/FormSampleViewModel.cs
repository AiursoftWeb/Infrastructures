using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Portal.Models.SamplesViewModels;

public class FormSampleViewModel
{
    [Required] public string Name { get; set; }

    [Required] [EmailAddress] public string Email { get; set; }

    [Required]
    [Display(Name = "Your Avatar")]
    public string IconAddress { get; set; }

    [Required]
    [Display(Name = "Your Homework")]
    public string HomeworkAddress { get; set; }
}
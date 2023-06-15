using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Portal.Models.ToolsViewModels;

public class Rot13ViewModel
{
    [Required]
    [Display(Name = "Source String")]
    public string SourceString { get; set; } = "Try any string!";

    [Display(Name = "Result String")] public string ResultString { get; set; }
}
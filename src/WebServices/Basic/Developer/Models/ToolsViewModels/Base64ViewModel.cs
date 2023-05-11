using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.ToolsViewModels;

public class Base64ViewModel
{
    [Required]
    [Display(Name = "Source String")]
    public string SourceString { get; set; } = "Try any string!";

    [Display(Name = "Result String")] public string ResultString { get; set; }

    public bool Decrypt { get; set; }
}
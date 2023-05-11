using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.ToolsViewModels;

public class QRCodeViewModel
{
    [Required]
    [Display(Name = "Source String")]
    public string SourceString { get; set; } = "Try any string!";
}
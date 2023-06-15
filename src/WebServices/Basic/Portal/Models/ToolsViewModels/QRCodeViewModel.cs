using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Portal.Models.ToolsViewModels;

public class QRCodeViewModel
{
    [Required]
    [Display(Name = "Source String")]
    public string SourceString { get; set; } = "Try any string!";
}
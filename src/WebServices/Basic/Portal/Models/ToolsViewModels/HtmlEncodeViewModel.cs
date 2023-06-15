using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Portal.Models.ToolsViewModels;

public class HtmlEncodeViewModel
{
    [Required]
    [Display(Name = "Source String")]
    public string SourceString { get; set; } = "<a href=\"something\">&</a>";

    [Display(Name = "Result String")] public string ResultString { get; set; }

    public bool Decrypt { get; set; }
}
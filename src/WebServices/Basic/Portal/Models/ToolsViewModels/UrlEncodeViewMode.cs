using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Portal.Models.ToolsViewModels;

public class UrlEncodeViewModel
{
    [Required]
    [Display(Name = "Source String")]
    public string SourceString { get; set; } = "?Email=anduin@aiursoft.com&password=M_ySuper(Strong)Pass[]@word";

    [Display(Name = "Result String")] public string ResultString { get; set; }

    public bool Decrypt { get; set; }
}
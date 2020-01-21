using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Developer.Models.ToolsViewModels
{
    public class JsonFormatViewModel
    {
        [Required]
        [Display(Name = "Source String")]
        public string SourceString { get; set; } = "{\"name\":\"your-name\" }";

        [Display(Name = "Result String")]
        public string ResultString { get; set; }

        public bool Format { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

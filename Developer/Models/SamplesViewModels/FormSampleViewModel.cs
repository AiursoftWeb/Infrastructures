using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.SamplesViewModels
{
    public class FormSampleViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Your Avatar")]
        public string IconAddress { get; set; }

        [Required]
        [Display(Name = "Your Homework")]
        public string HomeworkAddress { get; set; }

        public string DocumentHTML { get; set; }
    }
}

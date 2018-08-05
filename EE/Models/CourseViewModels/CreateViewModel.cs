using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.CourseViewModels
{
    public class CreateViewModel : CommonViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Describe your course about what will be talked about")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "What will be learned from this course")]
        public string WhatYouWillLearn { get; set; }
        public double Price { get; set; }
        public bool DisplayOwnerInfo { get; set; } = true;
    }
}

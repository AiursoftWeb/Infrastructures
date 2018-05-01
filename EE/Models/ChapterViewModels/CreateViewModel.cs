using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.ChapterViewModels
{
    public class CreateViewModel
    {
        public bool ModelStateValid { get; set; } = true;
        public string CourseName { get; set; }
        public int CourseId { get; set; }

        [Required]
        public string NewChapterTitle { get; set; }
    }
}

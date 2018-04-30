using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.ChapterViewModels
{
    public class CreateViewModel
    {
        public string CourseName { get; set; }
        public string NewChapterTitle { get; set; }
        public bool ModelStateValid { get; set; }
    }
}

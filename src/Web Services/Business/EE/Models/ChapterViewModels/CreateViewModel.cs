using System.ComponentModel.DataAnnotations;

namespace Aiursoft.EE.Models.ChapterViewModels
{
    public class CreateViewModel
    {
        public string CourseName { get; set; }
        public int CourseId { get; set; }

        [Required]
        public string NewChapterTitle { get; set; }
    }
}

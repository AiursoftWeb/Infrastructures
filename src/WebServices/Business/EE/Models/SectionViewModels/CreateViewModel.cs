using System.ComponentModel.DataAnnotations;

namespace Aiursoft.EE.Models.SectionViewModels
{
    public class CreateViewModel
    {
        [Required]
        [Display(Name = "New Section Name")]
        [MinLength(5)]
        [MaxLength(30)]
        public string NewSectionName { get; set; }
        public string CourseName { get; set; }
        public int CourseId { get; set; }
    }
}
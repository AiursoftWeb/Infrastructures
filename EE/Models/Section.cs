using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace Aiursoft.EE.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }

        public int SectionName { get; set; }

        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Context { get; set; }

        [InverseProperty(nameof(Chapter.Context))]
        public IEnumerable<Chapter> Chapters { get; set; }
    }
}
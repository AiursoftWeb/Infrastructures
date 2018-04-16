using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.EE.Models
{
    public class Chapter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VideoAddress { get; set; }
        public int ViewTimes { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsFree { get; set; }

        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
    }
}

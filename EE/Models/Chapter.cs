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

        public int SectionId { get; set; }
        [ForeignKey(nameof(SectionId))]
        public Section Context { get; set; }
    }
}

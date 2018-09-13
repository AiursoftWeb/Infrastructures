using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.EE.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public DateTime SubscribTime { get; set; } = DateTime.UtcNow;
        public bool Paid { get; set; } = false;

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public EEUser User { get; set; }

        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
    }
}

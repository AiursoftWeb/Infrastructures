using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public DateTime FollowTime { get; set; }

        public string TriggerId { get; set; }
        [ForeignKey(nameof(TriggerId))]
        public EEUser Trigger { get; set; }

        public string ReceiverId { get; set; }
        [ForeignKey(nameof(ReceiverId))]
        public EEUser Receiver { get; set; }
    }
}

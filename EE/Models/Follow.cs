using System;
using System.ComponentModel.DataAnnotations.Schema;

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

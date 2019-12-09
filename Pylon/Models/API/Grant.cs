using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.API
{
    public class Grant
    {
        public string AppID { get; set; }
        public DateTime GrantTime { get; set; } = DateTime.UtcNow;
        public string GatewayUserId { get; set; }
        [NotMapped]
        public virtual AiurUserBase UserInfo { get; set; }
    }
}

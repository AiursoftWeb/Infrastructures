using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Gateway.SDK.Models.API
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

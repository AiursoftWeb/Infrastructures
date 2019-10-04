using Aiursoft.Pylon.Models.API;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Gateway.Models
{
    public class AuditLogLocal : AuditLog
    {
        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public GatewayUser User { get; set; }
    }
}

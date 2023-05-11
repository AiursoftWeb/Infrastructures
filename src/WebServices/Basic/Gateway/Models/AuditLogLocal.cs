using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.Gateway.SDK.Models.API;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.Models;

public class AuditLogLocal : AuditLog
{
    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public GatewayUser User { get; set; }
}
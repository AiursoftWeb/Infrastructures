using Aiursoft.Pylon.Models.API;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Aiursoft.API.Models
{
    public class AuditLogLocal : AuditLog
    {
        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public APIUser User { get; set; }
    }
}

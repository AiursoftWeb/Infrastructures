using Aiursoft.Pylon.Models.API;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.API.Models
{
    public class AuditLogLocal : AuditLog
    {
        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public APIUser User { get; set; }
    }
}

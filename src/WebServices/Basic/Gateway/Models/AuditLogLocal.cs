using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.Directory.SDK.Models.API;
using Newtonsoft.Json;

namespace Aiursoft.Directory.Models;

public class AuditLogLocal : AuditLog
{
    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public DirectoryUser User { get; set; }
}
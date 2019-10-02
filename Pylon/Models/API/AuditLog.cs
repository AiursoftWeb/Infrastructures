using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Aiursoft.Pylon.Models.API
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public AiurUserBase User { get; set; }

        public DateTime HappenTime { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; }

        public string IPAddress { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.API.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public APIUser User { get; set; }

        public DateTime HappenTime { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; }

        public string IPAddress { get; set; }
    }
}

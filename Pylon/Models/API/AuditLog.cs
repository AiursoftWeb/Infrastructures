using System;

namespace Aiursoft.Pylon.Models.API
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string AppId { get; set; }

        public DateTime HappenTime { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; }

        public string IPAddress { get; set; }
    }
}

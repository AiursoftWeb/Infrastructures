using System;

namespace Aiursoft.Observer.Models
{
    public class MonitorRule
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string CheckAddress { get; set; }
        public bool LastHealthStatus { get; set; } = false;
        public string ExpectedContent { get; set; }
        public DateTime LastCheckTime { get; set; } = DateTime.UtcNow;
    }
}

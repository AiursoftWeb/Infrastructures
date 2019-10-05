namespace Aiursoft.Status.Models
{
    public class MonitorRule
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string CheckAddress { get; set; }
        public bool LastHealthStatus { get; set; } = false;
        public string ExpectedContent { get; set; }
    }
}

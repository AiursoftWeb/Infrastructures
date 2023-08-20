using Aiursoft.DbTools;

namespace Aiursoft.Status.Models;

public class MonitorRule : ISynchronizable<MonitorRule>
{
    public string ProjectName { get; set; }
    public string CheckAddress { get; set; }
    public bool LastHealthStatus { get; set; }
    public string ExpectedContent { get; set; }
    public DateTime LastCheckTime { get; set; } = DateTime.UtcNow;

    public bool EqualsInDb(MonitorRule obj)
    {
        return
            obj.ProjectName == ProjectName &&
            obj.CheckAddress == CheckAddress &&
            obj.ExpectedContent == ExpectedContent;
    }

    public MonitorRule Map()
    {
        return new MonitorRule
        {
            ProjectName = ProjectName,
            CheckAddress = CheckAddress,
            LastHealthStatus = false,
            ExpectedContent = ExpectedContent
        };
    }
}
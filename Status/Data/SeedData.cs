using Aiursoft.Status.Models;
using System.Collections.Generic;

namespace Aiursoft.Status.Data
{
    public static class SeedData
    {
        public static IEnumerable<MonitorRule> GetRules()
        {
            return new List<MonitorRule>
            {
                new MonitorRule
                {
                    ProjectName = "Aiursoft home page",
                    CheckAddress = "https://www.aiursoft.com/?show=direct",
                    ExpectedContent = "Free training, tools, and community to help you grow your skills, career, or business."
                }
            };
        }
    }
}

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
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Archon",
                    CheckAddress = "https://archon.aiursoft.com/",
                    ExpectedContent = "Welcome to Archon server!"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Developer",
                    CheckAddress = "https://developer.aiursoft.com/?show=direct",
                    ExpectedContent = "Welcome to Aiursoft Developer Center"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft authentication gateway",
                    CheckAddress = "https://gateway.aiursoft.com/oauth/authorize?appid=4b43b0aee35624cd95b910189b3dc231&redirect_uri=https%3A%2F%2Fdeveloper.aiursoft.com%2FAuth%2FAuthResult&state=%2F&scope=snsapi_base&response_type=code",
                    ExpectedContent = "Login to Aiursoft developer center"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Probe",
                    CheckAddress = "https://probe.aiursoft.com",
                    ExpectedContent = "Welcome to Probe!"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Stargate",
                    CheckAddress = "https://stargate.aiursoft.com",
                    ExpectedContent = "Welcome to Aiursoft Message queue server!"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft account center",
                    CheckAddress = "https://account.aiursoft.com/?show=direct",
                    ExpectedContent = "Unlock the power of your"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Colossus",
                    CheckAddress = "https://Colossus.aiursoft.com/?show=direct",
                    ExpectedContent = "Colossus produce a very pure online storage experience, which helps you share any file and transfer between any device."
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft wiki center",
                    CheckAddress = "https://wiki.aiursoft.com/Welcome/Home.md",
                    ExpectedContent = "Aiursoft Wiki Center"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Kahla",
                    CheckAddress = "https://server.kahla.app",
                    ExpectedContent = "Welcome to Aiursoft Kahla server!"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Tracer",
                    CheckAddress = "https://tracer.aiursoft.com",
                    ExpectedContent = "Tracer is a simple network quality testing app."
                },
            };
        }
    }
}

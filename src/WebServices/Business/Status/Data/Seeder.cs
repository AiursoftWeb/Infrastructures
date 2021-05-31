using Aiursoft.Archon.SDK.Services;
using Aiursoft.DBTools;
using Aiursoft.DBTools.Models;
using Aiursoft.Developer.SDK.Services;
using Aiursoft.Observer.SDK.Services;
using Aiursoft.Probe.SDK.Services;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Aiursoft.Stargate.SDK.Services;
using Aiursoft.Status.Models;
using Aiursoft.Warpgate.SDK.Services;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Status.Data
{
    public class Seeder : IScopedDependency, ISeeder
    {
        private readonly ServiceLocation serviceLocation;
        private readonly ObserverLocator observerLocator;
        private readonly StargateLocator stargateLocator;
        private readonly DeveloperLocator developerLocator;
        private readonly ArchonLocator archonLocator;
        private readonly ProbeLocator probeLocator;
        private readonly WarpgateLocator warpgateLocator;
        private readonly StatusDbContext dbContext;

        public Seeder(
            ServiceLocation serviceLocation,
            ObserverLocator observerLocator,
            StargateLocator stargateLocator,
            DeveloperLocator developerLocator,
            ArchonLocator archonLocator,
            ProbeLocator probeLocator,
            WarpgateLocator warpgateLocator,
            StatusDbContext dbContext)
        {
            this.serviceLocation = serviceLocation;
            this.observerLocator = observerLocator;
            this.stargateLocator = stargateLocator;
            this.developerLocator = developerLocator;
            this.archonLocator = archonLocator;
            this.probeLocator = probeLocator;
            this.warpgateLocator = warpgateLocator;
            this.dbContext = dbContext;
        }

        public void Seed()
        {
            dbContext.MonitorRules.Sync(GetRules().ToList());
            dbContext.SaveChanges();
        }

        public IEnumerable<MonitorRule> GetRules()
        {
            return new List<MonitorRule>
            {
                new MonitorRule
                {
                    ProjectName = "Aiursoft home page",
                    CheckAddress = $"{serviceLocation.WWW}/?show=direct",
                    ExpectedContent = "Free training, tools, and community to help you grow your skills, career, or business."
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Archon",
                    CheckAddress = $"{archonLocator.Endpoint}",
                    ExpectedContent = "Welcome to Archon server!"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Developer",
                    CheckAddress = $"{developerLocator.Endpoint}/?show=direct",
                    ExpectedContent = "Welcome to"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft authentication gateway",
                    CheckAddress = $"{serviceLocation.Gateway}/oauth/authorize?appid={AppsContainer.CurrentAppId}&redirect_uri=https%3A%2F%2Fwrong.aiursoft.com%2FAuth%2FAuthResult&state=%2F&scope=snsapi_base&response_type=code",
                    ExpectedContent = "This app can not use our authentication system"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Probe",
                    CheckAddress = probeLocator.Endpoint,
                    ExpectedContent = "Welcome to Probe!"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Stargate",
                    CheckAddress = stargateLocator.Endpoint,
                    ExpectedContent = "Welcome to"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Account Center",
                    CheckAddress = $"{serviceLocation.Account}/?show=direct",
                    ExpectedContent = "Unlock the power of your"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft wiki center",
                    CheckAddress = $"{serviceLocation.Wiki}/Welcome/Home.md",
                    ExpectedContent = "Wiki Center"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Observer",
                    CheckAddress = $"{observerLocator.Endpoint}",
                    ExpectedContent = "Welcome"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft Warpgate",
                    CheckAddress = warpgateLocator.Endpoint,
                    ExpectedContent = "Welcome"
                },
                new MonitorRule
                {
                    ProjectName = "Aiursoft UI",
                    CheckAddress = $"{serviceLocation.UI}/index.html",
                    ExpectedContent = "Welcome"
                }
            };
        }
    }
}

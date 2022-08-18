using Aiursoft.Archon.SDK.Services;
using Aiursoft.Developer.SDK.Services;
using Aiursoft.Observer.SDK.Services;
using Aiursoft.Probe.SDK.Services;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Aiursoft.Stargate.SDK.Services;
using Aiursoft.Status.Models;
using Aiursoft.Warpgate.SDK.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Status.Data
{
    public class MonitorDataProvider : ISingletonDependency
    {
        private readonly ServiceLocation serviceLocation;
        private readonly ObserverLocator observerLocator;
        private readonly StargateLocator stargateLocator;
        private readonly DeveloperLocator developerLocator;
        private readonly ArchonLocator archonLocator;
        private readonly ProbeLocator probeLocator;
        private readonly WarpgateLocator warpgateLocator;
        private readonly List<MonitorRule> customRules;

        public IReadOnlyCollection<MonitorRule> MonitorRules { get; init; }

        public MonitorDataProvider(
            ServiceLocation serviceLocation,
            ObserverLocator observerLocator,
            StargateLocator stargateLocator,
            DeveloperLocator developerLocator,
            ArchonLocator archonLocator,
            ProbeLocator probeLocator,
            WarpgateLocator warpgateLocator,
            IOptions<List<MonitorRule>> customRules)
        {
            this.serviceLocation = serviceLocation;
            this.observerLocator = observerLocator;
            this.stargateLocator = stargateLocator;
            this.developerLocator = developerLocator;
            this.archonLocator = archonLocator;
            this.probeLocator = probeLocator;
            this.warpgateLocator = warpgateLocator;
            this.customRules = customRules.Value;
            MonitorRules = BuildDefaultRules().ToArray();
        }

        private IEnumerable<MonitorRule> BuildDefaultRules()
        {
            var defaultList = new List<MonitorRule>
            {
                new()
                {
                    ProjectName = "Aiursoft Home Page",
                    CheckAddress = $"{serviceLocation.WWW}/?show=direct",
                    ExpectedContent = "Free training, tools, and community to help you grow your skills, career, or business."
                },
                new()
                {
                    ProjectName = "Aiursoft Archon",
                    CheckAddress = $"{archonLocator.Endpoint}",
                    ExpectedContent = "Welcome to Archon server!"
                },
                new()
                {
                    ProjectName = "Aiursoft Developer",
                    CheckAddress = $"{developerLocator.Endpoint}/?show=direct",
                    ExpectedContent = "Welcome to"
                },
                new()
                {
                    ProjectName = "Aiursoft authentication gateway",
                    CheckAddress = $"{serviceLocation.Gateway}/oauth/authorize?appid={AppsContainer.CurrentAppId}&redirect_uri=https%3A%2F%2Fwrong.aiursoft.com%2FAuth%2FAuthResult&state=%2F&scope=snsapi_base&response_type=code",
                    ExpectedContent = "This app can not use our authentication system"
                },
                new()
                {
                    ProjectName = "Aiursoft Probe",
                    CheckAddress = probeLocator.Endpoint,
                    ExpectedContent = "Welcome to Probe!"
                },
                new()
                {
                    ProjectName = "Aiursoft Stargate",
                    CheckAddress = stargateLocator.Endpoint,
                    ExpectedContent = "Welcome to"
                },
                new()
                {
                    ProjectName = "Aiursoft Account Center",
                    CheckAddress = $"{serviceLocation.Account}/?show=direct",
                    ExpectedContent = "Unlock the power of your"
                },
                new()
                {
                    ProjectName = "Aiursoft Wiki",
                    CheckAddress = $"{serviceLocation.Wiki}/Welcome/Home.md",
                    ExpectedContent = "Wiki Center"
                },
                new()
                {
                    ProjectName = "Aiursoft Observer",
                    CheckAddress = $"{observerLocator.Endpoint}",
                    ExpectedContent = "Welcome"
                },
                new()
                {
                    ProjectName = "Aiursoft Warpgate",
                    CheckAddress = warpgateLocator.Endpoint,
                    ExpectedContent = "Welcome"
                },
                new()
                {
                    ProjectName = "Aiursoft UI",
                    CheckAddress = $"{serviceLocation.UI}/index.html",
                    ExpectedContent = "Welcome"
                }
            };
            defaultList.AddRange(customRules);
            return defaultList;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Aiursoft.Scanner.Abstract;
using Aiursoft.SDK.Services;
using Aiursoft.Status.Models;
using Aiursoft.Warpgate.SDK.Services;
using Microsoft.Extensions.Options;
using Aiursoft.Observer.SDK.Configuration;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Stargate.SDK.Configuration;

namespace Aiursoft.Status.Data;

public class MonitorDataProvider : ISingletonDependency
{
    private readonly List<MonitorRule> customRules;
    private readonly ObserverConfiguration observerLocator;
    private readonly ProbeConfiguration probeLocator;
    private readonly DirectoryConfiguration directoryConfiguration;
    private readonly ServiceLocation serviceLocation;
    private readonly StargateConfiguration stargateLocator;
    private readonly WarpgateLocator warpgateLocator;

    public MonitorDataProvider(
        IOptions<DirectoryConfiguration> directoryConfiguration,
        IOptions<ObserverConfiguration> observerConfiguration,
        ServiceLocation serviceLocation,
        IOptions<StargateConfiguration> stargateLocator,
        IOptions<ProbeConfiguration> probeLocator,
        WarpgateLocator warpgateLocator,
        IOptions<List<MonitorRule>> customRules)
    {
        this.directoryConfiguration = directoryConfiguration.Value;
        this.observerLocator = observerConfiguration.Value;
        this.serviceLocation = serviceLocation;
        this.stargateLocator = stargateLocator.Value;
        this.probeLocator = probeLocator.Value;
        this.warpgateLocator = warpgateLocator;
        this.customRules = customRules.Value;
        MonitorRules = BuildDefaultRules().ToArray();
    }

    public IReadOnlyCollection<MonitorRule> MonitorRules { get; init; }

    private IEnumerable<MonitorRule> BuildDefaultRules()
    {
        var defaultList = new List<MonitorRule>
        {
            new()
            {
                ProjectName = "Aiursoft Home Page",
                CheckAddress = $"{serviceLocation.WWW}/?show=direct",
                ExpectedContent =
                    "Free training, tools, and community to help you grow your skills, career, or business."
            },
            new()
            {
                ProjectName = "Aiursoft Developer",

                // TODO: Developer will be merged to directory.
                CheckAddress = $"https://developer.aiursoft.com/?show=direct",
                ExpectedContent = "Welcome to"
            },
            new()
            {
                ProjectName = "Aiursoft Directory service",
                CheckAddress =
                    $"{directoryConfiguration.Instance}/oauth/authorize?appid={directoryConfiguration.AppId}&redirect_uri=https%3A%2F%2Fwrong.aiursoft.com%2FAuth%2FAuthResult&state=%2F&scope=snsapi_base&response_type=code",
                ExpectedContent = "This app can not use our authentication system"
            },
            new()
            {
                ProjectName = "Aiursoft Probe",
                CheckAddress = probeLocator.Instance,
                ExpectedContent = "Welcome to Probe!"
            },
            new()
            {
                ProjectName = "Aiursoft Stargate",
                CheckAddress = stargateLocator.Instance,
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
                CheckAddress = $"{observerLocator.Instance}",
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
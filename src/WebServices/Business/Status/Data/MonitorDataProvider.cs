using System.Collections.Generic;
using System.Linq;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Developer.SDK.Services;
using Aiursoft.Scanner.Abstract;
using Aiursoft.SDK.Services;
using Aiursoft.Stargate.SDK.Services;
using Aiursoft.Status.Models;
using Aiursoft.Warpgate.SDK.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Aiursoft.Observer.SDK.Configuration;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Directory.SDK.Configuration;

namespace Aiursoft.Status.Data;

public class MonitorDataProvider : ISingletonDependency
{
    private readonly DirectoryConfiguration gatewayLocator;
    private readonly List<MonitorRule> customRules;
    private readonly DeveloperLocator developerLocator;
    private readonly ObserverConfiguration observerLocator;
    private readonly ProbeConfiguration probeLocator;
    private readonly IConfiguration _configuration;
    private readonly ServiceLocation serviceLocation;
    private readonly StargateLocator stargateLocator;
    private readonly WarpgateLocator warpgateLocator;

    public MonitorDataProvider(
#warning Use Options!
#warning Deprecate IConfiguration!
        IConfiguration configuration,
        ServiceLocation serviceLocation,
        ObserverConfiguration observerLocator,
        StargateLocator stargateLocator,
        DeveloperLocator developerLocator,
        IOptions<DirectoryConfiguration> gatewayLocator,
        ProbeConfiguration probeLocator,
        WarpgateLocator warpgateLocator,
        IOptions<List<MonitorRule>> customRules)
    {
        _configuration = configuration;
        this.serviceLocation = serviceLocation;
        this.observerLocator = observerLocator;
        this.stargateLocator = stargateLocator;
        this.developerLocator = developerLocator;
        this.gatewayLocator = gatewayLocator.Value;
        this.probeLocator = probeLocator;
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
                CheckAddress = $"{developerLocator.Endpoint}/?show=direct",
                ExpectedContent = "Welcome to"
            },
            new()
            {
                ProjectName = "Aiursoft authentication gateway",
                CheckAddress =
                    $"{gatewayLocator.Instance}/oauth/authorize?appid={_configuration["AiursoftAppId"]}&redirect_uri=https%3A%2F%2Fwrong.aiursoft.com%2FAuth%2FAuthResult&state=%2F&scope=snsapi_base&response_type=code",
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
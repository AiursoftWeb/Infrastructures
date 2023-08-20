using Aiursoft.Scanner.Abstractions;
using Aiursoft.SDK.Services;
using Aiursoft.Status.Models;
using Microsoft.Extensions.Options;
using Aiursoft.Observer.SDK.Configuration;
using Aiursoft.Probe.SDK.Configuration;
using Aiursoft.Directory.SDK.Configuration;
using Aiursoft.Stargate.SDK.Configuration;
using Aiursoft.Warpgate.SDK.Configuration;

namespace Aiursoft.Status.Data;

public class MonitorDataProvider : ISingletonDependency
{
    private readonly List<MonitorRule> _customRules;
    private readonly ObserverConfiguration _observerLocator;
    private readonly ProbeConfiguration _probeLocator;
    private readonly DirectoryConfiguration _directoryConfiguration;
    private readonly ServiceLocation _serviceLocation;
    private readonly StargateConfiguration _stargateLocator;
    private readonly WarpgateConfiguration _warpgateLocator;

    public MonitorDataProvider(
        IOptions<DirectoryConfiguration> directoryConfiguration,
        IOptions<ObserverConfiguration> observerConfiguration,
        ServiceLocation serviceLocation,
        IOptions<StargateConfiguration> stargateLocator,
        IOptions<ProbeConfiguration> probeLocator,
        IOptions<WarpgateConfiguration> warpgateLocator,
        IOptions<List<MonitorRule>> customRules)
    {
        _directoryConfiguration = directoryConfiguration.Value;
        _observerLocator = observerConfiguration.Value;
        _serviceLocation = serviceLocation;
        _stargateLocator = stargateLocator.Value;
        _probeLocator = probeLocator.Value;
        _warpgateLocator = warpgateLocator.Value;
        _customRules = customRules.Value;
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
                CheckAddress = $"{_serviceLocation.WWW}/?show=direct",
                ExpectedContent =
                    "Free training, tools, and community to help you grow your skills, career, or business."
            },
            new()
            {
                ProjectName = "Aiursoft Directory service",
                CheckAddress =
                    $"{_directoryConfiguration.Instance}/oauth/authorize?appid={_directoryConfiguration.AppId}&redirect_uri=https%3A%2F%2Fwrong.aiursoft.com%2FAuth%2FAuthResult&state=%2F&scope=snsapi_base&response_type=code",
                ExpectedContent = "This app can not use our authentication system"
            },
            new()
            {
                ProjectName = "Aiursoft Probe",
                CheckAddress = _probeLocator.Instance,
                ExpectedContent = "Welcome to Probe!"
            },
            new()
            {
                ProjectName = "Aiursoft Stargate",
                CheckAddress = _stargateLocator.Instance,
                ExpectedContent = "Welcome to"
            },
            new()
            {
                ProjectName = "Aiursoft Account Center",
                CheckAddress = $"{_serviceLocation.Account}/?show=direct",
                ExpectedContent = "Unlock the power of your"
            },
            new()
            {
                ProjectName = "Aiursoft Wiki",
                CheckAddress = $"{_serviceLocation.Wiki}/Welcome/Home.md",
                ExpectedContent = "Wiki Center"
            },
            new()
            {
                ProjectName = "Aiursoft Observer",
                CheckAddress = $"{_observerLocator.Instance}",
                ExpectedContent = "Welcome"
            },
            new()
            {
                ProjectName = "Aiursoft Warpgate",
                CheckAddress = _warpgateLocator.Instance,
                ExpectedContent = "Welcome"
            },
            new()
            {
                ProjectName = "Aiursoft UI",
                CheckAddress = $"{_serviceLocation.UI}/index.html",
                ExpectedContent = "Welcome"
            }
        };
        defaultList.AddRange(_customRules);
        return defaultList;
    }
}
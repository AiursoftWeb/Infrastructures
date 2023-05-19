using Aiursoft.Gateway.SDK.Services;
using Aiursoft.SDK.Tests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aiursoft.Observer.Tests;

public class TestStartup : Startup
{
    public TestStartup(IConfiguration configuration) : base(configuration)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.RemoveAll<ACTokenValidator>();
        services.AddTransient<ACTokenValidator, MockAcTokenValidator>();
    }
}
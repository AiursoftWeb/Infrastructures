using Aiursoft.Directory.SDK.Services;
using Aiursoft.SDK.Tests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aiursoft.Warpgate.Tests;

public class TestStartup : Startup
{
    public TestStartup(IConfiguration configuration) : base(configuration)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.RemoveAll<AiursoftAppTokenValidator>();
        services.AddTransient<AiursoftAppTokenValidator, MockAcTokenValidator>();
    }
}
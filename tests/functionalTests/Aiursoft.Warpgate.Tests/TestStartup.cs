using Aiursoft.Directory.SDK.Services;
using Aiursoft.SDK.Tests.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aiursoft.Warpgate.Tests;

public class TestStartup : Startup
{
    public override void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
    {
        base.ConfigureServices(configuration, environment, services);
        services.RemoveAll<AiursoftAppTokenValidator>();
        services.AddTransient<AiursoftAppTokenValidator, MockAcTokenValidator>();
    }
}
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Archon.Tests.Services;
using Aiursoft.Developer.SDK.Services.ToDeveloperServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Archon.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.RemoveAll<DeveloperApiService>();
            services.AddTransient<DeveloperApiService, MockDeveloperApiService>();
        }
    }
}

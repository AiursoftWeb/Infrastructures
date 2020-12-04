using Aiursoft.Archon.SDK.Services;
using Aiursoft.Observer.Tests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Observer.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.RemoveAll<ACTokenValidator>();
            services.AddTransient<ACTokenValidator, MockAcTokenValidator>();
        }
    }
}

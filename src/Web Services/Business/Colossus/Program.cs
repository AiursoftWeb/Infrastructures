using Aiursoft.Archon.SDK.Services;
using Aiursoft.Colossus.Data;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Colossus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDbContext<ColossusDbContext>()
                .InitSite<AppsContainer>(c => c["ColossusPublicSiteName"], a => a.AccessToken())
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}

using Aiursoft.Archon.SDK.Services;
using Aiursoft.Developer.Data;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Developer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args)
                .Update<DeveloperDbContext>()
                .InitSite<AppsContainer>(c => c["AppsIconSiteName"], a => a.AccessToken())
                .Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

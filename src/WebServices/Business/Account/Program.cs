using Aiursoft.Account.Data;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Account
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args)
                .Update<AccountDbContext>()
                .InitSite<AppsContainer>(c => c["UserIconSiteName"], a => a.AccessToken())
                .Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

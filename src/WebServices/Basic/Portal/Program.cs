using Aiursoft.Portal.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;
using System.Threading.Tasks;

namespace Aiursoft.Portal;

public class Program
{
    public static async Task Main(string[] args)
    {
        await App<Startup>(args)
            .Update<PortalDbContext>()
            .RunAsync();
        
        // await (await App<Startup>(args)
        //     .Update<PortalDbContext>()
        //     .InitSite<AppsContainer>(c => c["AppsIconSiteName"], a => a.GetAccessTokenAsync()))
        //     .RunAsync();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
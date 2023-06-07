using Aiursoft.Directory.SDK.Services;
using Aiursoft.Developer.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;
using Aiursoft.Probe.SDK;
using System.Threading.Tasks;

namespace Aiursoft.Developer;

public class Program
{
    public static async Task Main(string[] args)
    {
        await (await App<Startup>(args)
            .Update<DeveloperDbContext>()
            .InitSite<AppsContainer>(c => c["AppsIconSiteName"], a => a.GetAccessTokenAsync()))
            .RunAsync();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
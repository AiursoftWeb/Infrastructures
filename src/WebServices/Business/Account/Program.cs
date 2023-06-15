using Aiursoft.Account.Data;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Probe.SDK;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Account;

public class Program
{
    public static async Task Main(string[] args)
    {
        await (await (await App<Startup>(args)
            .UpdateDbAsync<AccountDbContext>())
            .InitSiteAsync<AppsContainer>(c => c["UserIconSiteName"], a => a.GetAccessTokenAsync()))
            .RunAsync();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
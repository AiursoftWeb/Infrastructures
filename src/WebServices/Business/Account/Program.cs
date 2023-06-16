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
        var app = App<Startup>(args);
        await app.UpdateDbAsync<AccountDbContext>();
        await app.InitSiteAsync<AppsContainer>(c => c["UserIconSiteName"], a => a.GetAccessTokenAsync());
        await app.RunAsync();
    }
}
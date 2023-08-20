using Aiursoft.Account.Data;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Probe.SDK;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Account;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<AccountDbContext>(UpdateMode.MigrateThenUse);
        await app.InitSiteAsync<DirectoryAppTokenService>(c => c["UserIconSiteName"], a => a.GetAccessTokenAsync());
        await app.RunAsync();
    }
}
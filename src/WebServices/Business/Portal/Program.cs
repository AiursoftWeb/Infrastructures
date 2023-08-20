using Aiursoft.Portal.Data;
using static Aiursoft.WebTools.Extends;
using Aiursoft.DbTools;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Probe.SDK;

namespace Aiursoft.Portal;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<PortalDbContext>(UpdateMode.MigrateThenUse);
        await app.InitSiteAsync<DirectoryAppTokenService>(c => c["AppsIconSiteName"], a => a.GetAccessTokenAsync());
        await app.RunAsync();
    }
}
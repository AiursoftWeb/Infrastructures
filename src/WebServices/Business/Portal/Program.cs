using Aiursoft.Portal.Data;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;
using System.Threading.Tasks;
using Aiursoft.DbTools;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Probe.SDK;

namespace Aiursoft.Portal;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<PortalDbContext>();
        await app.InitSiteAsync<DirectoryAppTokenService>(c => c["AppsIconSiteName"], a => a.GetAccessTokenAsync());
        await app.RunAsync();
    }
}
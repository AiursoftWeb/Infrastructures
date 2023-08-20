using Aiursoft.Probe.Data;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Probe;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<ProbeDbContext>(UpdateMode.MigrateThenUse);
        await app.RunAsync();
    }
}
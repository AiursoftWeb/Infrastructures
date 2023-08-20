using Aiursoft.Wiki.Data;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Wiki;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<WikiDbContext>(UpdateMode.MigrateThenUse);
        await app.SeedAsync();
        await app.RunAsync();
    }
}
using Aiursoft.Configuration.Data;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<ConfigurationDbContext>(UpdateMode.MigrateThenUse);
        await app.RunAsync();
    }
}
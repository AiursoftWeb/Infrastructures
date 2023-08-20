using Aiursoft.Directory.Data;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Directory;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<DirectoryDbContext>(UpdateMode.MigrateThenUse);
        // TODO: There should create new Probe Site. But that logic should be in a new project called Gateway.
        await app.RunAsync();
    }
}
using Aiursoft.Stargate.Data;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Stargate;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<StargateDbContext>(UpdateMode.MigrateThenUse);
        await app.RunAsync();
    }
}
using Aiursoft.Observer.Data;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Observer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<ObserverDbContext>(UpdateMode.MigrateThenUse);
        await app.RunAsync();
    }
}
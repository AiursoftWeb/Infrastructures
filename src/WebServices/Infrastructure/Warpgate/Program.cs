using Aiursoft.SDK;
using Aiursoft.Warpgate.Data;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Warpgate;

public class Program
{
    public static async Task Main(string[] args)
    {
        (await App<Startup>(args).UpdateDbAsync<WarpgateDbContext>()).Run();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
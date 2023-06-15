using Aiursoft.SDK;
using Aiursoft.Wiki.Data;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Wiki;

public class Program
{
    public static async Task Main(string[] args)
    {
        (await (await App<Startup>(args).UpdateDbAsync<WikiDbContext>()).SeedAsync()).Run();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
using Aiursoft.Configuration.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        (await App<Startup>(args).UpdateDbAsync<ConfigurationDbContext>()).Run();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
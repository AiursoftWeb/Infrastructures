using Aiursoft.Probe.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Probe;

public class Program
{
    public static async Task Main(string[] args)
    {
        (await App<Startup>(args).UpdateDbAsync<ProbeDbContext>()).Run();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
using Aiursoft.EE.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.EE;

public class Program
{
    public static async Task Main(string[] args)
    {
        (await App<Startup>(args).UpdateDbAsync<EEDbContext>()).Run();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
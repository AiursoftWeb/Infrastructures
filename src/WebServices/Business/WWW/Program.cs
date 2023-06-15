using Aiursoft.SDK;
using Aiursoft.WWW.Data;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.WWW;

public class Program
{
    public static async Task Main(string[] args)
    {
        (await App<Startup>(args).UpdateDbAsync<WWWDbContext>()).Run();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
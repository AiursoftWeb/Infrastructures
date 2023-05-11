using Aiursoft.SDK;
using Aiursoft.Wiki.Data;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Wiki;

public class Program
{
    public static void Main(string[] args)
    {
        App<Startup>(args)
            .Update<WikiDbContext>()
            .Seed()
            .Run();
    }

    // For EF
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return BareApp<Startup>(args);
    }
}
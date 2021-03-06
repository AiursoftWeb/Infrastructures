using Aiursoft.SDK;
using Aiursoft.Warpgate.Data;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Warpgate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<WarpgateDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

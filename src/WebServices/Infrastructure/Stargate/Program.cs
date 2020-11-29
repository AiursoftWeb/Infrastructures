using Aiursoft.SDK;
using Aiursoft.Stargate.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Stargate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<StargateDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

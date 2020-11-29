using Aiursoft.Gateway.Data;
using Aiursoft.SDK;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<GatewayDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

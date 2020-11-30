using Aiursoft.Probe.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Probe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<ProbeDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

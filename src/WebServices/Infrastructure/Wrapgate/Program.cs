using Aiursoft.SDK;
using Aiursoft.Wrapgate.Data;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Wrapgate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<WrapgateDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

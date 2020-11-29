using Aiursoft.SDK;
using Aiursoft.Wrap.Data;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Wrap
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<WrapDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}


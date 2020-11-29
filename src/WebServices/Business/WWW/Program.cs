using Aiursoft.SDK;
using Aiursoft.WWW.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.WWW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<WWWDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

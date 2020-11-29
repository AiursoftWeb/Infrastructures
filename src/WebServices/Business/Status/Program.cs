using Aiursoft.SDK;
using Aiursoft.Status.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Status
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<StatusDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

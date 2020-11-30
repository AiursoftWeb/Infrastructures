using Aiursoft.Observer.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Observer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<ObserverDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

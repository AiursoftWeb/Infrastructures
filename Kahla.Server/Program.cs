using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace Kahla.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                 .UseStartup<Startup>()
                 .UseUrls("http://*:5005")
                 .Build();

            return host;
        }
    }
}

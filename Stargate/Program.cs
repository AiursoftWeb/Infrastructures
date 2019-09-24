using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Stargate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildHost(args).Run();
        }

        public static IHost BuildHost(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(t => t.UseStartup<Startup>())
                 .Build();

            return host;
        }
    }
}

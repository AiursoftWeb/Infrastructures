using Aiursoft.API.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Aiursoft.Pylon;

namespace Aiursoft.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<APIDbContext>()
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                 .UseApplicationInsights()
                 .UseStartup<Startup>()
                 .Build();

            return host;
        }
    }
}

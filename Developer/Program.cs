using Aiursoft.Developer.Data;
using Aiursoft.Pylon;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Aiursoft.Developer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<DeveloperDbContext>()
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
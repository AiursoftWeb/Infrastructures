using Aiursoft.Colossus.Data;
using Aiursoft.Pylon;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Aiursoft.Colossus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<ColossusDbContext>(null)
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();
    }
}

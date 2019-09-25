using Aiursoft.Probe.Data;
using Aiursoft.Pylon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Probe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDbContext<ProbeDbContext>()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}

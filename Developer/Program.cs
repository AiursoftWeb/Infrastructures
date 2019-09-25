using Aiursoft.Developer.Data;
using Aiursoft.Pylon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Developer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDbContext<DeveloperDbContext>()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}
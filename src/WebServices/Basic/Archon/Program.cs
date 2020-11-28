using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Aiursoft.Archon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildHost(args)
                .Run();
        }

        public static IHost BuildHost(string[] args, int port = -1)
        {
            return CreateHostBuilder(args, port)
                .Build();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, int port)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (port > 0)
                    {
                        webBuilder.UseUrls($"http://localhost:{port}");
                    }
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}

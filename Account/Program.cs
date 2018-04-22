using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Aiursoft.Pylon;
using Aiursoft.Account.Data;

namespace Aiursoft.Account
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
            .MigrateDbContext<AccountDbContext>()
            .Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                 .UseStartup<Startup>()
                 .Build();

            return host;
        }
    }
}

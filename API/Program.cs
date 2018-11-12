using Aiursoft.API.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Aiursoft.Pylon;
using System.Security.Cryptography;

namespace Aiursoft.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rsa = new RSACryptoServiceProvider();
            var key = rsa.ExportParameters(true);
            var text = Newtonsoft.Json.JsonConvert.SerializeObject(key);

            BuildWebHost(args)
                .MigrateDbContext<APIDbContext>((db, services) => db.Seed(services))
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

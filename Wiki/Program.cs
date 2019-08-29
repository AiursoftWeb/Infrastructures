﻿using Aiursoft.Pylon;
using Aiursoft.Wiki.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Aiursoft.Wiki
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<WikiDbContext>()
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

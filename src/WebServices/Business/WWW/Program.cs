using Aiursoft.SDK;
using Aiursoft.WWW.Data;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.WWW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).MigrateDbContext<WWWDbContext>().Run();
        }
    }
}

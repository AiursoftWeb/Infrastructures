using Aiursoft.Configuration.Data;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<ConfigurationDbContext>();
        await app.RunAsync();
    }
}
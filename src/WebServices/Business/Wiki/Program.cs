using Aiursoft.SDK;
using Aiursoft.Wiki.Data;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Wiki;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<WikiDbContext>();
        await app.SeedAsync();
        await app.RunAsync();
    }
}
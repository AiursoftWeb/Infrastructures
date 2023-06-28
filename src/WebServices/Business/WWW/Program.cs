using Aiursoft.WWW.Data;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.WWW;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<WWWDbContext>();
        await app.RunAsync();
    }
}
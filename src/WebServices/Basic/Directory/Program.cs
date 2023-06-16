using Aiursoft.Directory.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Directory;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<DirectoryDbContext>();
        // TODO: There should create new Probe Site. But that logic should be in a new project called Gateway.
        await app.RunAsync();
    }
}
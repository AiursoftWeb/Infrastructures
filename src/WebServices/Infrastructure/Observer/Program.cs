using Aiursoft.Observer.Data;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Observer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<ObserverDbContext>();
        await app.RunAsync();
    }
}
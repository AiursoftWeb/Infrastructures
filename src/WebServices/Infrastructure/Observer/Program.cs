using Aiursoft.Observer.Data;
using Aiursoft.SDK;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
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
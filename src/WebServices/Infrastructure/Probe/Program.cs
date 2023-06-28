using Aiursoft.Probe.Data;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Probe;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.UpdateDbAsync<ProbeDbContext>();
        await app.RunAsync();
    }
}
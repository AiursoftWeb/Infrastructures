using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Status;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = App<Startup>(args);
        await app.RunAsync();
    }
}
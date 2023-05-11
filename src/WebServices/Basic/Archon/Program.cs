using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Archon;

public class Program
{
    public static void Main(string[] args)
    {
        App<Startup>(args).Run();
    }
}
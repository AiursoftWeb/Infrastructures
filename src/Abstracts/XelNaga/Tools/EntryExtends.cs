using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tools
{
    public static class EntryExtends
    {
        public static bool IsInEF()
        {
            return Assembly.GetEntryAssembly().GetName().Name.ToLower().Trim().StartsWith("ef");
        }

        public static bool IsInUT()
        {
            return Assembly.GetEntryAssembly().GetName().Name.ToLower().Trim().StartsWith("test");
        }

        public static bool IsProgramEntry(bool log = true)
        {
            var inEF = IsInEF();
            var inUT = IsInUT();
            if (log)
            {
                var textEF = inEF ? "Entity Framework" : string.Empty;
                var textUT = inUT ? "Unit Test" : string.Empty;
                var program = Assembly.GetEntryAssembly().GetName().Name;
                Console.WriteLine($"Environment status: [{textEF}{textUT}] [{program}]");
                if (inEF || inUT)
                {
                    Console.Beep();
                    Console.WriteLine("This environment is only for development!");
                }
            }
            return !inEF && !inUT;
        }
    }
}

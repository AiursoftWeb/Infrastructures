using System;
using System.Reflection;

namespace Aiursoft.XelNaga.Tools
{
    public static class EntryExtends
    {
        private static bool? _cache;

        private static bool IsInEF()
        {
            return Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Trim().StartsWith("ef") ?? false;
        }

        private static bool IsInUT()
        {
            var name = Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Trim() ?? string.Empty; 
            return name.StartsWith("test") || name.StartsWith("resharpertestrunner");
        }

        public static bool IsProgramEntry(bool log = true)
        {
            if (_cache != null)
            {
                return _cache.Value;
            }
            var inEF = IsInEF();
            var inUT = IsInUT();
            if (log)
            {
                var textEF = inEF ? "Entity Framework" : string.Empty;
                var textUT = inUT ? "Unit Test" : string.Empty;
                var program = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
                Console.WriteLine($"Environment status: [{textEF}{textUT}] [{program}]");
                if (inEF || inUT)
                {
                    Console.Beep();
                    Console.WriteLine("This environment is only for development!");
                }
            }
            _cache = !inEF && !inUT;
            return _cache.Value;
        }
    }
}

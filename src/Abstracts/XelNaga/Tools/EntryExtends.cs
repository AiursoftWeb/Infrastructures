using System.Reflection;

namespace Aiursoft.XelNaga.Tools;

public static class EntryExtends
{
    private static bool? _isProgramCache;

    public static bool IsInEntityFramework()
    {
        return Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Trim().StartsWith("ef") ?? false;
    }

    public static bool IsInUnitTests()
    {
        var name = Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Trim() ?? string.Empty;
        return name.StartsWith("test") || name.EndsWith("testrunner");
    }

    public static bool IsProgramEntry()
    {
        if (_isProgramCache != null)
        {
            return _isProgramCache.Value;
        }

        _isProgramCache = !IsInEntityFramework() && !IsInUnitTests();
        return _isProgramCache.Value;
    }
}
using System.Reflection;

namespace Aiursoft.XelNaga.Tools;

public static class EntryExtends
{
    private static bool? _isProgramCache;

    private static bool IsInEF()
    {
        return Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Trim().StartsWith("ef") ?? false;
    }

    public static bool IsInUT()
    {
        var name = Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Trim() ?? string.Empty;
        return name.StartsWith("test") || name.EndsWith("testrunner");
    }

    public static bool IsProgramEntry()
    {
        if (_isProgramCache != null) return _isProgramCache.Value;
        _isProgramCache = !IsInEF() && !IsInUT();
        return _isProgramCache.Value;
    }
}
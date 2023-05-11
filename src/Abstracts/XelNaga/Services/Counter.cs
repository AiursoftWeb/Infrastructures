using System.Threading;
using Aiursoft.Scanner.Interfaces;

namespace Aiursoft.XelNaga.Services;

public class Counter : ISingletonDependency
{
    private int _current;

    /// <summary>
    ///     Last returned counter value. If a initial counter, will be -1.
    /// </summary>
    public int GetCurrent => _current;

    /// <summary>
    ///     Get a new scope unique number which is one larger than current.
    /// </summary>
    /// <returns></returns>
    public int GetUniqueNo()
    {
        return Interlocked.Increment(ref _current);
    }
}
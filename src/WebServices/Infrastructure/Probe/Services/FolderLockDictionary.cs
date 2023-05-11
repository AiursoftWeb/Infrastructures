using System.Collections.Generic;
using System.Threading;
using Aiursoft.Scanner.Interfaces;

namespace Aiursoft.Probe.Services;

public class FolderLockDictionary : ISingletonDependency
{
    private readonly Dictionary<int, SemaphoreSlim> _dictionary = new();

    public SemaphoreSlim GetLock(int contextId)
    {
        lock (this)
        {
            if (_dictionary.TryGetValue(contextId, out var result))
            {
                return result;
            }

            var newLock = new SemaphoreSlim(1, 1);
            _dictionary[contextId] = newLock;
            return newLock;
        }
    }
}
using Aiursoft.Scanner.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace Aiursoft.Probe.Services
{
    public class FolderLockDictionary : ISingletonDependency
    {
        private Dictionary<int, SemaphoreSlim> _dictionary = new Dictionary<int, SemaphoreSlim>();

        public SemaphoreSlim GetLock(int contextId)
        {
            lock (this)
            {
                if (_dictionary.ContainsKey(contextId))
                {
                    return _dictionary[contextId];
                }
                else
                {
                    var newLock = new SemaphoreSlim(1, 1);
                    _dictionary[contextId] = newLock;
                    return newLock;
                }
            }
        }
    }

}

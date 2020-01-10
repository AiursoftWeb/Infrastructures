using Aiursoft.XelNaga.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Aiursoft.Stargate.Services
{
    public class ConnectedCountService : ISingletonDependency
    {
        private readonly IMemoryCache _memoryCache;

        public ConnectedCountService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void AddConnectedCount(int channelId)
        {
            if (_memoryCache.TryGetValue($"Connected-{channelId}", out int count))
            {
                _memoryCache.Set($"Connected-{channelId}", count + 1);
                return;
            }
            _memoryCache.Set($"Connected-{channelId}", 1);
        }

        public void ReduceConnectedCount(int channelId)
        {
            if (_memoryCache.TryGetValue($"Connected-{channelId}", out int count))
            {
                _memoryCache.Set($"Connected-{channelId}", count - 1);
                return;
            }
            _memoryCache.Set($"Connected-{channelId}", 0);
        }

        public int GetConnectedCount(int channelId)
        {
            if (_memoryCache.TryGetValue($"Connected-{channelId}", out int count))
            {
                return count;
            }
            return 0;
        }
    }
}

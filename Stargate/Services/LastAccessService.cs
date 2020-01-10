using Aiursoft.XelNaga.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Aiursoft.Stargate.Services
{
    public class LastAccessService : ISingletonDependency
    {
        private readonly IMemoryCache _memoryCache;

        public LastAccessService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void RecordLastConnectTime(int channelId)
        {
            _memoryCache.Set($"LastConnect-{channelId}", DateTime.UtcNow);
        }

        public DateTime GetLastAccessTime(int channelId)
        {
            if (_memoryCache.TryGetValue($"LastConnect-{channelId}", out DateTime connectTime))
            {
                return connectTime;
            }
            return DateTime.MinValue;
        }
    }
}

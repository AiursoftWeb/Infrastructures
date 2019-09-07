using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class AiurCache
    {
        private readonly IMemoryCache _cache;

        public AiurCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAndCache<T>(string cacheKey, Func<Task<T>> backup)
        {
            if (!_cache.TryGetValue(cacheKey, out T resultValue))
            {
                resultValue = await backup();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(20));

                _cache.Set(cacheKey, resultValue, cacheEntryOptions);
            }
            return resultValue;
        }
    }
}

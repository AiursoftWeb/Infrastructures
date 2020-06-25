using Aiursoft.Scanner.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Services
{
    public class AiurCache : ITransientDependency
    {
        private readonly IMemoryCache _cache;

        public AiurCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAndCache<T>(string cacheKey, Func<Task<T>> backup, int cachedMinutes = 20)
        {
            if (!_cache.TryGetValue(cacheKey, out T resultValue) || resultValue == null)
            {
                resultValue = await backup();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cachedMinutes));

                _cache.Set(cacheKey, resultValue, cacheEntryOptions);
            }
            return resultValue;
        }

        public void Clear(string key)
        {
            _cache.Remove(key);
        }

        public T GetAndCache<T>(string cacheKey, Func<T> backup, int cachedMinutes = 20)
        {
            if (!_cache.TryGetValue(cacheKey, out T resultValue) || resultValue == null)
            {
                resultValue = backup();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cachedMinutes));

                _cache.Set(cacheKey, resultValue, cacheEntryOptions);
            }
            return resultValue;
        }
    }
}

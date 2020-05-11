using Aiursoft.Scanner.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Aiursoft.SDK.Services
{
    public class AiurCache : ITransientDependency
    {
        private readonly IMemoryCache _cache;

        public AiurCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAndCacheWhen<T>(string cacheKey, Func<Task<T>> backup, Func<T, bool> when)
        {
            if (!_cache.TryGetValue(cacheKey, out T objectInCache))
            {
                // Object not in cache.
                objectInCache = await backup();
                _cache.Set(cacheKey, objectInCache, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(20)));
                return objectInCache;
            }
            else
            {
                // Object in cache.
                var dontTrustCache = !when(objectInCache);
                if (dontTrustCache)
                {
                    // Subfolder is not in cache. Might because cache is out dated!
                    var resultReal = await backup();
                    _cache.Set(cacheKey, resultReal, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(20)));
                    return resultReal;
                }
                else
                {
                    // Subfolder is not in cache. And cache refreshed. Return null.
                    return objectInCache;
                }
            }
        }

        public async Task<T> GetAndCache<T>(string cacheKey, Func<Task<T>> backup, int cachedMinutes = 20)
        {
            if (!_cache.TryGetValue(cacheKey, out T resultValue))
            {
                resultValue = await backup();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cachedMinutes));

                _cache.Set(cacheKey, resultValue, cacheEntryOptions);
            }
            return resultValue;
        }

        public T GetAndCache<T>(string cacheKey, Func<T> backup, int cachedMinutes = 20)
        {
            if (!_cache.TryGetValue(cacheKey, out T resultValue))
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

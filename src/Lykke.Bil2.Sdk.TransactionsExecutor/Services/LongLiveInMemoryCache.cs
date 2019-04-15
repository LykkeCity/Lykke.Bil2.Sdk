using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    internal class LongLiveInMemoryCache
    {
        private readonly MemoryCache _cache;
        private readonly SemaphoreSlim _guard;

        public LongLiveInMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(10),
                Clock = new SystemClock()
            });
            _guard = new SemaphoreSlim(1, 1);
        }

        public async Task<TItem> GetOrAddAsync<TItem>(string key, TimeSpan absoluteExpirationRelativeToNow, Func<Task<TItem>> itemFactory)
        {
            if (_cache.TryGetValue<TItem>(key, out var result))
            {
                return result;
            }

            await _guard.WaitAsync();

            try
            {
                if (_cache.TryGetValue(key, out result))
                {
                    return result;
                }

                result = await itemFactory();

                _cache.Set(key, result, absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
            }
            finally
            {
                _guard.Release();
            }

            return result;
        }
    }
}

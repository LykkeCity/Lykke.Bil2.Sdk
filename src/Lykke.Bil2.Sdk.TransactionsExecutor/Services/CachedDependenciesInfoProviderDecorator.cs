using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    internal class CachedDependenciesInfoProviderDecorator : IDependenciesInfoProvider
    {
        private readonly IDependenciesInfoProvider _inner;
        private readonly LongLiveInMemoryCache _cache;
        private readonly TimeSpan _cacheExpirationPeriod;

        public CachedDependenciesInfoProviderDecorator(
            IDependenciesInfoProvider inner,
            LongLiveInMemoryCache cache,
            TimeSpan cacheExpirationPeriod)
        {
            _inner = inner;
            _cache = cache;
            _cacheExpirationPeriod = cacheExpirationPeriod;
            
        }

        public Task<IReadOnlyDictionary<DependencyName, DependencyInfo>> GetInfoAsync()
        {
            const string key = nameof(CachedDependenciesInfoProviderDecorator);

            return _cache.GetOrAddAsync(key, _cacheExpirationPeriod, () => _inner.GetInfoAsync());
        }
    }
}

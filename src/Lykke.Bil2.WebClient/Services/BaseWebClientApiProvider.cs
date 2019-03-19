using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.Services
{
    /// <inheritdoc />
    [PublicAPI]
    public abstract class BaseWebClientApiProvider<TApi> : IBaseWebClientApiProvider<TApi>
    {
        private readonly IReadOnlyDictionary<string, TApi> _integrationApis;

        protected BaseWebClientApiProvider(IReadOnlyDictionary<string, TApi> integrationApis)
        {
            _integrationApis = integrationApis;
        }

        /// <inheritdoc />
        public TApi GetApi(string integrationName)
        {
            if (!_integrationApis.TryGetValue(integrationName, out var api))
            {
                throw new InvalidOperationException($"API for the integration {integrationName} is not found");
            }

            return api;
        }
    }
}

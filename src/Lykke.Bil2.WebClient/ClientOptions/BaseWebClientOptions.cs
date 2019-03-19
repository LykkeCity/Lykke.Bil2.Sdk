using System;
using System.Collections.Generic;
using System.Net.Http;
using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.ClientOptions
{
    /// <summary>
    /// Base web client options.
    /// </summary>
    [PublicAPI]
    public class BaseWebClientOptions<TIntegrationOptions>
    
        where TIntegrationOptions : BaseWebClientIntegrationOptions, new()
    {
        /// <summary>
        /// Specifies timeout for the HTTP requests. Null means default timeout.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        internal IReadOnlyCollection<DelegatingHandler> DelegatingHandlers => _delegatingHandlers;

        internal IReadOnlyDictionary<string, TIntegrationOptions> IntegrationOptions => _integrationOptions;

        private readonly List<DelegatingHandler> _delegatingHandlers;
        private readonly Dictionary<string, TIntegrationOptions> _integrationOptions;

        public BaseWebClientOptions()
        {
            _delegatingHandlers = new List<DelegatingHandler>();
            _integrationOptions = new Dictionary<string, TIntegrationOptions>();
        }

        /// <summary>
        /// Ads HTTP requests delegating handler.
        /// </summary>
        /// <param name="handler"></param>
        public void AddDelegatingHandler(DelegatingHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _delegatingHandlers.Add(handler);
        }

        /// <summary>
        /// Adds and configures blockchain integration.
        /// </summary>
        public void AddIntegration(string integrationName, Action<TIntegrationOptions> configureIntegration)
        {
            if (string.IsNullOrWhiteSpace(integrationName))
            {
                throw new ArgumentException("Should be not empty string", nameof(integrationName));
            }
            if (configureIntegration == null)
            {
                throw new ArgumentNullException(nameof(configureIntegration));
            }

            var integrationOptions = new TIntegrationOptions();

            configureIntegration.Invoke(integrationOptions);

            if (string.IsNullOrWhiteSpace(integrationOptions.Url))
            {
                throw new InvalidOperationException($"Url should be specified for the integration {integrationName}");
            }
            if (!Uri.TryCreate(integrationOptions.Url, UriKind.Absolute, out _))
            {
                throw new InvalidOperationException($"URL should be specified for the integration {integrationName}, actual value: {integrationOptions.Url}");
            }

            _integrationOptions.Add(integrationName, integrationOptions);
        }
    }
}

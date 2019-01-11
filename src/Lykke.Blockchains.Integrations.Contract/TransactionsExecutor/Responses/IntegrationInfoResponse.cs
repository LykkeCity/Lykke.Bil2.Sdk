using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [GET] /api/integration-info
    /// </summary>
    [PublicAPI]
    public class IntegrationInfoResponse
    {
        /// <summary>
        /// Configuration of the integration. Should contain
        /// all settings. Secure settings should be
        /// sanitized. For instance connection string to the storage
        /// is secure setting and password/access token should be replaced with
        /// some kind of dummy text.
        /// </summary>
        [JsonProperty("configuration")]
        public IDictionary<string, string> Configuration { get; }

        /// <summary>
        /// Blockchain specific information.
        /// </summary>
        [JsonProperty("blockchain")]
        public BlockchainInfo Blockchain { get; }

        /// <summary>
        /// Info concerning services on which integration is dependent. This should include node and all intermediate APIs.
        /// </summary>
        [JsonProperty("dependencies")]
        public IDictionary<string, DependencyInfo> Dependencies { get; }

        public IntegrationInfoResponse(
            IDictionary<string, string> configuration,
            BlockchainInfo blockchain,
            IDictionary<string, DependencyInfo> dependencies)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }
    }
}

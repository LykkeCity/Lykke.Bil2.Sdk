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
            BlockchainInfo blockchain,
            IDictionary<string, DependencyInfo> dependencies)
        {
            Blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }
    }
}

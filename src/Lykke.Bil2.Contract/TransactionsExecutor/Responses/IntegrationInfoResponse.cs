using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
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
        public IReadOnlyDictionary<string, DependencyInfo> Dependencies { get; }

        /// <summary>
        /// Endpoint: [GET] /api/integration-info
        /// </summary>
        /// <param name="blockchain">Blockchain specific information.</param>
        /// <param name="dependencies">
        /// Info concerning services on which integration is dependent. This should include node and all intermediate APIs.
        /// </param>
        public IntegrationInfoResponse(
            BlockchainInfo blockchain,
            IReadOnlyDictionary<string, DependencyInfo> dependencies)
        {
            Blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }
    }
}

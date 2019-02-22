using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Models
{
    /// <summary>
    /// Contains information about running integration
    /// </summary>
    [PublicAPI]
    public class IntegrationInfo
    {
        /// <summary>
        /// Blockchain specific information.
        /// </summary>
        public BlockchainInfo Blockchain { get; }

        /// <summary>
        /// Info concerning services on which integration is dependent. This should include node and all intermediate APIs.
        /// </summary>
        public IDictionary<string, DependencyInfo> Dependencies { get; }

        public IntegrationInfo(
            BlockchainInfo blockchain,
            IDictionary<string, DependencyInfo> dependencies)
        {
            Blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }
    }
}

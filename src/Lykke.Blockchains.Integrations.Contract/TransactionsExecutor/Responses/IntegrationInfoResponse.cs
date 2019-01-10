using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        public IDictionary<string, string> Configuration { get; set; }

        /// <summary>
        /// Blockchain specific information.
        /// </summary>
        [JsonProperty("blockchain")]
        public BlockchainInfo Blockchain { get; set; }

        /// <summary>
        /// Info concerning services on which integration is dependent. This should include node and all intermediate APIs.
        /// </summary>
        [JsonProperty("dependencies")]
        public IDictionary<string, DependencyInfo> Dependencies { get; set; }

        public class BlockchainInfo
        {
            /// <summary>
            /// Number of the latest available block in the blockchain according to the integration.
            /// </summary>
            [JsonProperty("latestBlockNumber")]
            public long LatestBlockNumber { get; set; }

            /// <summary>
            /// Moment of the latest available block in the blockchain according to the integration.
            /// </summary>
            [JsonProperty("latestBlockMoment")]
            public DateTime LatestBlockMoment { get; set; }
        }

        public class DependencyInfo
        {
            /// <summary>
            /// Running version of the dependency.
            /// </summary>
            [JsonProperty("runningVersion")]
            [JsonConverter(typeof(VersionConverter))]
            public Version RunningVersion { get; set; }

            /// <summary>
            /// Latest available version of the dependency. 
            /// </summary>
            [JsonProperty("latestAvailableVersion")]
            [JsonConverter(typeof(VersionConverter))]
            public Version LatestAvailableVersion { get; set; }
        }
    }
}

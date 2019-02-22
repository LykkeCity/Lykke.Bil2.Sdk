using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Responses;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <inheritdoc />
    [PublicAPI]
    public class TransactionsExecutorIsAliveResponse : BlockchainIsAliveResponse
    {
        /// <summary>
        /// Optional.
        /// Should describe the problems if integration is unhealthy.
        /// For instance implementation could check 
        /// if node and all used intermediate APIs are running 
        /// well and they are consistent, configuration is 
        /// correct and all required dependencies are accessible.
        /// </summary>
        [CanBeNull]
        [JsonProperty("disease")]
        public string Disease { get; }

        public TransactionsExecutorIsAliveResponse(
            string name, 
            Version version, 
            string envInfo, 
            Version contractVersion, 
            string disease = null) : 

            base
            (
                name, 
                version, 
                envInfo,
                contractVersion
            )
        {
            if(disease != null && string.IsNullOrWhiteSpace(disease))
                throw new ArgumentException("Disease should be either null or not empty string");

            Disease = disease;
        }
    }
}

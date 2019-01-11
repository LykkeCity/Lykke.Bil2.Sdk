using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction input.
    /// </summary>
    [PublicAPI]
    public class Input
    {
        /// <summary>
        /// Asset ID to transfer.
        /// </summary>
        [JsonProperty("assetId")]
        public string AssetId { get; }

        /// <summary>
        /// Address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; }
        
        /// <summary>
        /// Amount to transfer from the given address.
        /// </summary>
        [JsonProperty("amount")]
        public CoinsAmount Amount { get; }

        /// <summary>
        /// Optional.
        /// Address context associated with the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressContext")]
        public string AddressContext { get; }

        public Input(
            string assetId, 
            string address, 
            CoinsAmount amount,
            string addressContext = null)
        {
            if (string.IsNullOrWhiteSpace(assetId))
                throw new ArgumentException("Should be not empty string", nameof(assetId));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Should be not empty string", nameof(address));

            if(amount == 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Should be positive number");

            AssetId = assetId;
            Address = address;
            Amount = amount;
            AddressContext = addressContext;
        }
    }
}

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Received coin.
    /// </summary>
    [PublicAPI, DataContract]
    public class ReceivedCoin
    {
        /// <summary>
        /// Number of received coin in the transaction.
        /// </summary>
        [DataMember(Order = 0)]
        public int CoinNumber { get; }

        /// <summary>
        /// Asset of the coin.
        /// </summary>
        [DataMember(Order = 1)]
        public Asset Asset { get; }

        /// <summary>
        /// Value of the coin.
        /// </summary>
        [DataMember(Order = 2)]
        public UMoney Value { get; }

        /// <summary>
        /// Optional.
        /// Address which received the coin.
        /// </summary>
        [CanBeNull, DataMember(Order = 3)]
        public Address Address { get; }

        /// <summary>
        /// Optional.
        /// Tag of the receiving address.
        /// </summary>
        [CanBeNull, DataMember(Order = 4)]
        public AddressTag AddressTag { get; }

        /// <summary>
        /// Optional.
        /// Type of the receiving address tag.
        /// </summary>
        [CanBeNull, DataMember(Order = 5)]
        public AddressTagType? AddressTagType { get; }
        
        /// <summary>
        /// Optional.
        /// Nonce number of the transaction for the receiving address.
        /// </summary>
        [CanBeNull, DataMember(Order = 6)]
        public long? AddressNonce { get; }

        /// <summary>
        /// Received coin.
        /// </summary>
        /// <param name="coinNumber">Number of received coin in the transaction.</param>
        /// <param name="asset">Asset of the coin.</param>
        /// <param name="value">Value of the coin.</param>
        /// <param name="address">
        /// Optional.
        /// Address which received the coin.
        /// </param>
        /// <param name="addressTag">
        /// Optional.
        /// Tag of the receiving address.
        /// </param>
        /// <param name="addressTagType">
        /// Optional.
        /// Type of the receiving address tag.
        /// </param>
        /// <param name="addressNonce">
        /// Optional.
        /// Nonce number of the transaction for the receiving address.
        /// </param>
        public ReceivedCoin(
            int coinNumber,
            Asset asset,
            UMoney value,
            Address address = null,
            AddressTag addressTag = null,
            AddressTagType? addressTagType = null,
            long? addressNonce = null)
        {
            if (coinNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(coinNumber), coinNumber, "Should be zero or positive number");

            if (addressTag != null && address == null)
                throw new ArgumentException("If the tag is specified, the address should be specified too");

            if (addressTagType.HasValue && addressTag == null)
                throw new ArgumentException("If the tag type is specified, the tag should be specified too");

            CoinNumber = coinNumber;
            Asset = asset ?? throw new ArgumentNullException(nameof(asset));
            Value = value;
            Address = address;
            AddressTag = addressTag;
            AddressTagType = addressTagType;
            AddressNonce = addressNonce;
        }
    }
}

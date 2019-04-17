using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Change of the address balance made by a transaction
    /// </summary>
    [PublicAPI, DataContract]
    public class BalanceChange
    {
        /// <summary>
        /// ID of the transfer within the transaction.
        /// Can group several balance changing operations into the single transfer,
        /// or can be just the output number.
        /// </summary>
        [DataMember(Order = 0)]
        public string TransferId { get; }

        /// <summary>
        /// Asset.
        /// </summary>
        [DataMember(Order = 1)]
        public Asset Asset { get; }

        /// <summary>
        /// Value for which the balance of the address was changed.
        /// Can be positive to increase the balance or negative to decrease the balance.
        /// </summary>
        [DataMember(Order = 2)]
        public Money Value { get; }

        /// <summary>
        /// Optional.
        /// Address.
        /// </summary>
        [DataMember(Order = 3)]
        public Address Address { get; }

        /// <summary>
        /// Optional.
        /// Tag of the address.
        /// </summary>
        [CanBeNull, DataMember(Order = 4)]
        public AddressTag Tag { get; }

        /// <summary>
        /// Optional.
        /// Type of the address tag.
        /// </summary>
        [CanBeNull, DataMember(Order = 5)]
        public AddressTagType? TagType { get; }

        /// <summary>
        /// Optional.
        /// Nonce number of the transaction for the address.
        /// </summary>
        [CanBeNull, DataMember(Order = 6)]
        public long? Nonce { get; }

        /// <summary>
        /// Change of the address balance made by a transaction
        /// </summary>
        /// <param name="transferId">
        /// ID of the transfer within the transaction.
        /// Can group several balance changing operations into the single transfer,
        /// or can be just the output number.
        /// </param>
        /// <param name="asset">Asset.</param>
        /// <param name="value">
        /// Value for which the balance of the address was changed.
        /// Can be positive to increase the balance or negative to decrease the balance.
        /// </param>
        /// <param name="address">
        /// Optional.
        /// Address.
        /// </param>
        /// <param name="tag">
        /// Optional.
        /// Tag of the address.
        /// </param>
        /// <param name="tagType">
        /// Optional.
        /// Type of the address tag.
        /// </param>
        /// <param name="nonce">
        /// Optional.
        /// Nonce number of the transaction for the address.
        /// </param>
        public BalanceChange(
            string transferId, 
            Asset asset, 
            Money value, 
            Address address = null, 
            AddressTag tag = null, 
            AddressTagType? tagType = null,
            long? nonce = null)
        {           
            if (string.IsNullOrWhiteSpace(transferId))
                throw new ArgumentException("Should be not empty string", nameof(transferId));

            if (tag != null && address == null)
                throw new ArgumentException("If the tag is specified, the address should be specified too");

            if (tagType.HasValue && tag == null)
                throw new ArgumentException("If the tag type is specified, the tag should be specified too");

            TransferId = transferId;
            Asset = asset ?? throw new ArgumentNullException(nameof(asset));
            Value = value;
            Address = address;
            Tag = tag;
            TagType = tagType;
            Nonce = nonce;
        }
    }
}

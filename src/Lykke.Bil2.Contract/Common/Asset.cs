using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common
{
    /// <summary>
    /// Asset.
    /// </summary>
    [PublicAPI]
    public sealed class Asset :
        IComparable<Asset>,
        IEquatable<Asset>
    {
        /// <summary>
        /// ID of the asset.
        /// </summary>
        [JsonProperty("id")]
        public AssetId Id { get; }

        /// <summary>
        /// Optional.
        /// Address of the asset.
        /// </summary>
        [CanBeNull]
        [JsonProperty("address")]
        public AssetAddress Address { get; }

        /// <summary>
        /// Assert
        /// </summary>
        /// <param name="id">ID of the asset.</param>
        /// <param name="address">
        /// Optional.
        /// Address of the asset.
        /// </param>
        public Asset(AssetId id, AssetAddress address = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Address = address;
        }

        public override string ToString()
        {
            return Address != null ? $"{Id}:{Address}" : Id.ToString();
        }

        public int CompareTo(Asset other)
        {
            if (ReferenceEquals(this, other)) 
                return 0;
            if (ReferenceEquals(null, other)) 
                return 1;
            
            var idComparison = Comparer<AssetId>.Default.Compare(Id, other.Id);
            if (idComparison != 0) 
                return idComparison;
            
            return Comparer<AssetAddress>.Default.Compare(Address, other.Address);
        }

        public bool Equals(Asset other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return Equals(Id, other.Id) && Equals(Address, other.Address);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            return obj is Asset asset && Equals(asset);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Address != null ? Address.GetHashCode() : 0);
            }
        }
    }
}

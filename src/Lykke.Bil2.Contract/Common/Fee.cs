using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Numerics;

namespace Lykke.Bil2.Contract.Common
{
    /// <summary>
    /// Fee amount.
    /// </summary>
    [PublicAPI]
    public sealed class Fee : 
        IComparable<Fee>,
        IEquatable<Fee>
    {
        /// <summary>
        /// Asset of the fee.
        /// </summary>
        public Asset Asset { get; }

        /// <summary>
        /// Amount of the fee.
        /// </summary>
        public UMoney Amount { get; }

        /// <summary>
        /// Fee amount.
        /// </summary>
        /// <param name="asset">Asset of the fee.</param>
        /// <param name="amount">Amount of the fee.</param>
        public Fee(Asset asset, UMoney amount)
        {
            Asset = asset ?? throw new ArgumentNullException(nameof(asset));
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{Asset} = {Amount}";
        }

        public int CompareTo(Fee other)
        {
            if (ReferenceEquals(this, other)) 
                return 0;
            if (ReferenceEquals(null, other)) 
                return 1;
            
            var assetComparison = Comparer<Asset>.Default.Compare(Asset, other.Asset);
            if (assetComparison != 0) 
                return assetComparison;
            
            return Amount.CompareTo(other.Amount);
        }

        public bool Equals(Fee other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return Equals(Asset, other.Asset) && Amount.Equals(other.Amount);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            return obj is Fee fee && Equals(fee);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Asset != null ? Asset.GetHashCode() : 0) * 397) ^ Amount.GetHashCode();
            }
        }
    }
}

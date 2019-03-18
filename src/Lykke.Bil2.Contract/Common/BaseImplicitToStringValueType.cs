using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.JsonConverters;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common
{
    [PublicAPI]
    [JsonConverter(typeof(ImplicitToStringJsonConverter))]
    public abstract class BaseImplicitToStringValueType<TConcrete> :
        IComparable<TConcrete>,
        IEquatable<TConcrete>

        where TConcrete : BaseImplicitToStringValueType<TConcrete>
    {
        /// <summary>
        /// The value
        /// </summary>
        public string Value { get; }

        protected BaseImplicitToStringValueType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Should be not empty string", nameof(value));

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public int CompareTo(TConcrete other)
        {
            if (ReferenceEquals(this, other)) 
                return 0;
            if (ReferenceEquals(null, other)) 
                return 1;
            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }

        public bool Equals(TConcrete other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            return obj is TConcrete concreteObject && Equals(concreteObject);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }
    }
}

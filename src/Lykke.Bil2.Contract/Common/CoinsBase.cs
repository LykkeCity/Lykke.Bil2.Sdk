using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Numerics.Money;

namespace Lykke.Bil2.Contract.Common
{
    /// <summary>
    /// Base coins class.
    /// </summary>
    [PublicAPI]
    public abstract class CoinsBase
    {
        private readonly Money _value;

        protected CoinsBase(Money value, bool allowNegative)
        {
            if (!allowNegative && value < Money.Zero)
                throw new CoinsConversionException("Negative values are not allowed", value);
            
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        protected static T Parse<T>(string value, Func<Money, T> factory)
            where T : CoinsBase
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            
            if (!Money.TryParse(value, out var moneyValue))
                throw new CoinsConversionException("Value can't be parsed as Money", value);

            return factory.Invoke(moneyValue);
        }
        
        public static implicit operator Money(CoinsBase value)
        {
            return value._value;
        }
    }
}

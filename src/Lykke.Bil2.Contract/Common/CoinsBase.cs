using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Lykke.Bil2.Contract.Common
{
    /// <summary>
    /// Base coins class.
    /// </summary>
    [PublicAPI]
    public abstract class CoinsBase
    {
        [CanBeNull]
        public string StringValue { get; }

        private NumberStyles _numberStyle;

        protected CoinsBase(string stringValue, bool allowNegative)
        {
            if (stringValue == null)
                throw new ArgumentNullException(nameof(stringValue));

            _numberStyle = allowNegative
                ? NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign
                : NumberStyles.AllowDecimalPoint;

            if (!decimal.TryParse(stringValue, _numberStyle, CultureInfo.InvariantCulture, out var decimalValue))
                throw new CoinsConversionException("Value can't be parsed as decimal", stringValue);

            if (!allowNegative && decimalValue < 0)
                throw new CoinsConversionException("Negative values are not allowed", stringValue);

            StringValue = stringValue;
        }

        protected static string DecimalToString(decimal value, int accuracy)
        {
            if (accuracy < 0 || accuracy > 28)
                throw new CoinsConversionException($"Asset accuracy should be number in the range [0..28], but is [{accuracy}]", value);
            
            var roundedValue = Math.Round(value, accuracy);

            return roundedValue.ToString($"0.{new string('0', accuracy)}", CultureInfo.InvariantCulture);
        }

        public static implicit operator decimal(CoinsBase value)
        {
            return value.ToDecimal();
        }

        public decimal ToDecimal()
        {
            return decimal.Parse(StringValue, _numberStyle, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return StringValue;
        }
    }
}

using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// Coins amount - positive number with fixed accuracy.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(CoinsAmountJsonConverter))]
    public sealed class CoinsAmount : CoinsBase
    {
        private CoinsAmount(string stringValue) : 
            base(stringValue, allowNegative: false)
        {
        }

        public static CoinsAmount Create(string stringValue)
        {
            return new CoinsAmount(stringValue);
        }

        public static CoinsAmount FromDecimal(decimal value, int accuracy)
        {
            if (value < 0)
                throw new CoinsConversionException("Only zero and positive values are allowed", value);

            var stringValue = DecimalToString(value, accuracy);

            return new CoinsAmount(stringValue);
        }
    }
}

using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.JsonConverters;
using Lykke.Numerics.Money;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common
{
    /// <summary>
    /// Coins amount - positive number with fixed accuracy.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(CoinsAmountJsonConverter))]
    public sealed class CoinsAmount : CoinsBase
    {
        public CoinsAmount(Money value) : 
            base(value, allowNegative: false)
        {
        }

        public static CoinsAmount Parse(string value)
            => Parse(value, v => new CoinsAmount(v));
    }
}

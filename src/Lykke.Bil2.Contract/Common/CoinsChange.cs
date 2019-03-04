using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.JsonConverters;
using Lykke.Numerics.Money;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common
{
    /// <summary>
    /// Coins change - positive or negative number with fixed accuracy.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(CoinsChangeJsonConverter))]
    public sealed class CoinsChange : CoinsBase
    {
        public CoinsChange(Money value) : 
            base(value, allowNegative: true)
        {
        }

        public static CoinsChange Parse(string value)
            => Parse(value, v => new CoinsChange(v));
    }
}

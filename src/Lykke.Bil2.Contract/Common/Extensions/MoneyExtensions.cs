using Lykke.Numerics.Money;

namespace Lykke.Bil2.Contract.Common.Extensions
{
    public static class MoneyExtensions
    {
        public static CoinsAmount ToCoinsAmount(this Money value)
        {
            return new CoinsAmount(value);
        }
        
        public static CoinsChange ToCoinsChange(this Money value)
        {
            return new CoinsChange(value);
        }
    }
}

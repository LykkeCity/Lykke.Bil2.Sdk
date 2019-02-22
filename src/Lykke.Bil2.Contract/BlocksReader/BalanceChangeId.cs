using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;

namespace Lykke.Bil2.Contract.BlocksReader
{
    [PublicAPI]
    public sealed class BalanceChangeId : BaseImplicitToStringValueType<BalanceChangeId>
    {
        public BalanceChangeId(string value) :
            base(value)
        {
        }

        public static implicit operator BalanceChangeId(string value)
        {
            return value != null ? new BalanceChangeId(value) : null;
        }
   
        public static implicit operator string(BalanceChangeId value)
        {
            return value?.Value;
        }
    }
}

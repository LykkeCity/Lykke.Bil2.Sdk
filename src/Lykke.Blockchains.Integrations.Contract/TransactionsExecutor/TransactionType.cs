using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor
{
    /// <summary>
    /// Enum describing implementation specific type of the transaction.
    /// </summary>
    [PublicAPI]
    public enum TransactionType
    {
        /// <summary>
        /// Corresponds to the “sending” transaction, for example in Nano blockchain.
        /// </summary>
        [EnumMember(Value = "sending")]
        Sending,

        /// <summary>
        /// Corresponds to the “receiving” transaction, for example in Nano blockchain.
        /// </summary>
        [EnumMember(Value = "receiving ")]
        Receiving
    }
}

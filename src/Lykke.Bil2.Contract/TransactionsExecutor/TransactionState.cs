using System.Runtime.Serialization;

namespace Lykke.Bil2.Contract.TransactionsExecutor
{
    /// <summary>
    /// Enum describing state of the transaction in the blockchain.
    /// </summary>
    public enum TransactionState
    {
        /// <summary>
        /// Transaction is unknown.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown,

        /// <summary>
        /// Transaction is accepted by the node, but not broadcasted yet to the network.
        /// </summary>
        [EnumMember(Value = "accepted")]
        Accepted,

        /// <summary>
        /// Transaction is broadcasted, but not included into a block.
        /// </summary>
        [EnumMember(Value = "broadcasted")]
        Broadcasted,

        /// <summary>
        /// Transaction is broadcasted and included into a block.
        /// </summary>
        [EnumMember(Value = "mined")]
        Mined
    }
}

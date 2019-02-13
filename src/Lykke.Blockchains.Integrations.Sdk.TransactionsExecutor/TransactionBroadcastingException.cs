using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor
{
    [PublicAPI]
    public class TransactionBroadcastingException : BlockchainIntegrationException
    {
        public TransactionBroadcastingError Error { get; }

        public TransactionBroadcastingException(TransactionBroadcastingError error, string message) : 
            base(message)
        {
            Error = error;
        }

        public TransactionBroadcastingException(TransactionBroadcastingError error, string message, Exception inner) : 
            base(message, inner)
        {
            Error = error;
        }
    }
}

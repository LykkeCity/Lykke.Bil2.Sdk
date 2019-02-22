using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor;
using Lykke.Blockchains.Integrations.Sdk.Exceptions;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Exceptions
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

using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Sdk.Exceptions;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions
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

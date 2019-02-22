using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Sdk.Exceptions;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions
{
    [PublicAPI]
    public class SendingTransactionBuildingException : BlockchainIntegrationException
    {
        public SendingTransactionBuildingError Error { get; }

        public SendingTransactionBuildingException(SendingTransactionBuildingError error, string message) : 
            base(message)
        {
            Error = error;
        }

        public SendingTransactionBuildingException(SendingTransactionBuildingError error, string message, Exception inner) : 
            base(message, inner)
        {
            Error = error;
        }
    }
}

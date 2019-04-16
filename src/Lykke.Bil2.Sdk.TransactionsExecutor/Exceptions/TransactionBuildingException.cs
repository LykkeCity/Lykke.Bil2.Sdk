using System;
using JetBrains.Annotations;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions
{
    [PublicAPI]
    public class TransactionBuildingException : BlockchainIntegrationException
    {
        public TransactionBuildingError Error { get; }

        public TransactionBuildingException(TransactionBuildingError error, string message) : 
            base(message)
        {
            Error = error;
        }

        public TransactionBuildingException(TransactionBuildingError error, string message, Exception inner) : 
            base(message, inner)
        {
            Error = error;
        }
    }
}

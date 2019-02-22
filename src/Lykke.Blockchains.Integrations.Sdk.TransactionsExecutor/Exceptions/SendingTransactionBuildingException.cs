using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor;
using Lykke.Blockchains.Integrations.Sdk.Exceptions;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Exceptions
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

using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor
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

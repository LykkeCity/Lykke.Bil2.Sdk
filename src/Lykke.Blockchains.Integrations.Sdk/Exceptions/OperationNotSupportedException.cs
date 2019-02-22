using System;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Sdk.Exceptions
{
    /// <summary>
    /// Exception indicating that operation is not supported by the integration.
    /// </summary>
    [PublicAPI]
    public class OperationNotSupportedException : BlockchainIntegrationException
    {
        public OperationNotSupportedException() :
            this("Operation is not supported by the blockchain integration")
        {
        }

        public OperationNotSupportedException(string message) : 
            base(message)
        {
        }

        public OperationNotSupportedException(string message, Exception inner) : 
            base(message, inner)
        {
        }
    }
}

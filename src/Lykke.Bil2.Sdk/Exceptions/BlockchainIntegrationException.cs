using System;
using JetBrains.Annotations;

namespace Lykke.Bil2.Sdk.Exceptions
{
    /// <summary>
    /// Base blockchain integration exception 
    /// </summary>
    [PublicAPI]
    public class BlockchainIntegrationException : Exception
    {
        public BlockchainIntegrationException(string message) :
            base(message)
        {
        }

        public BlockchainIntegrationException(string message, Exception inner) :
            base(message, inner)
        {
        }
    }
}

using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Sdk.SignService.Services;

namespace Lykke.Blockchains.Integrations.Sdk.SignService
{
    /// <summary>
    /// Service provider options for a blockchain integration sign service.
    /// </summary>
    [PublicAPI]
    public class SignServiceOptions
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain integration.
        /// </summary>
        public string IntegrationName { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IAddressGenerator"/> implementation factory.
        /// </summary>
        public Func<IServiceProvider, IAddressGenerator> AddressGeneratorFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="ITransactionSigner"/> implementation factory.
        /// </summary>
        public Func<IServiceProvider, ITransactionSigner> TransactionSignerFactory { get;set; }

        
    }
}

using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services;
using Lykke.Blockchains.Integrations.Sdk.BlocksReader.Settings;
using Lykke.SettingsReader;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader
{
    /// <summary>
    /// Service provider options for a blockchain integration blocks reader.
    /// </summary>
    [PublicAPI]
    public class BlocksReaderServiceOptions<TAppSettings>

        where TAppSettings : IBlocksReaderSettings<BaseBlocksReaderDbSettings>
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain integration. Words should be separated by the space, each word should be started from the capital case.
        /// </summary>
        public string IntegrationName { get; set; }

        /// <summary>
        /// Optional.
        /// Provides options to access application settings.
        /// </summary>
        [CanBeNull]
        public Action<IReloadingManager<TAppSettings>> UseSettings { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IBlockReader"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, IBlockReader> BlockReaderFactory { get; set; }

        internal IrreversibleBlockRetrievingStrategy IrreversbielBlockRetrievingStrategy { get; private set; }

        internal Func<ServiceFactoryContext<TAppSettings>, IIrreversibleBlockProvider> IrreversibleBlockProviderFactory { get; private set; }

        /// <summary>
        /// Adds irreversible blocks support with pulling strategy.
        /// </summary>
        public void AddIrreversibleBlockPulling(Func<ServiceFactoryContext<TAppSettings>, IIrreversibleBlockProvider> irreversibleBlockProviderFactory)
        {
            if (IrreversbielBlockRetrievingStrategy != IrreversibleBlockRetrievingStrategy.NotSupported)
            {
                throw new InvalidOperationException($"Irreversible block retrieving strategy has been already set to {IrreversbielBlockRetrievingStrategy}");
            }

            IrreversbielBlockRetrievingStrategy = IrreversibleBlockRetrievingStrategy.Pulling;
            IrreversibleBlockProviderFactory = irreversibleBlockProviderFactory ?? throw new ArgumentNullException(nameof(irreversibleBlockProviderFactory));
        }

        /// <summary>
        /// Adds irreversible blocks support with pushing strategy.
        /// </summary>
        /// <remarks>
        /// Use <see cref="IIrreversibleBlockListener"/> to notify about irreversible block updates.
        /// </remarks>
        public void AddIrreversibleBlockPushing()
        {
            if (IrreversbielBlockRetrievingStrategy != IrreversibleBlockRetrievingStrategy.NotSupported)
            {
                throw new InvalidOperationException($"Irreversible block retrieving strategy has been already set to {IrreversbielBlockRetrievingStrategy}");
            }

            IrreversbielBlockRetrievingStrategy = IrreversibleBlockRetrievingStrategy.Pushing;
        }
    }
}

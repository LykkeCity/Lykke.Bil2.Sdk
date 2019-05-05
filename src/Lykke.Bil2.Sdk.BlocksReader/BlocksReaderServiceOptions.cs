using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.Sdk.BlocksReader.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Sdk.BlocksReader
{
    /// <summary>
    /// Service provider options for a blockchain integration blocks reader.
    /// </summary>
    [PublicAPI]
    public class BlocksReaderServiceOptions<TAppSettings>

        where TAppSettings : IBlocksReaderSettings<BaseBlocksReaderDbSettings, BaseBlocksReaderRabbitMqSettings>
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain integration in CamelCase
        /// </summary>
        public string IntegrationName { get; set; }

        /// <summary>
        /// Optional.
        /// RabbitMq Vhost
        /// </summary>
        public string RabbitVhost { get; set; }

        /// <summary>
        /// Optional.
        /// Provides options to access application settings.
        /// </summary>
        [CanBeNull]
        public Action<IServiceCollection, IReloadingManager<TAppSettings>> UseSettings { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IBlockReader"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, IBlockReader> BlockReaderFactory { get; set; }

        internal BlockchainTransferModel TransferModel { get; private set; }

        internal bool IsTransferModelSet { get; private set; }

        internal IrreversibleBlockRetrievingStrategy IrreversibleBlockRetrievingStrategy { get; private set; }

        internal Func<ServiceFactoryContext<TAppSettings>, IIrreversibleBlockProvider> IrreversibleBlockProviderFactory { get; private set; }

        /// <summary>
        /// Sets "transfer coins" transactions model.
        /// </summary>
        public void UseTransferCoinsTransactionsModel()
        {
            TransferModel = BlockchainTransferModel.Coins;
            IsTransferModelSet = true;
        }

        /// <summary>
        /// Sets "transfer amount" transactions model.
        /// </summary>
        public void UseTransferAmountTransactionsModel()
        {
            TransferModel = BlockchainTransferModel.Amount;
            IsTransferModelSet = true;
        }

        /// <summary>
        /// Adds irreversible blocks support with pulling strategy.
        /// </summary>
        public void AddIrreversibleBlockPulling(Func<ServiceFactoryContext<TAppSettings>, IIrreversibleBlockProvider> irreversibleBlockProviderFactory)
        {
            if (IrreversibleBlockRetrievingStrategy != IrreversibleBlockRetrievingStrategy.NotSupported)
            {
                throw new InvalidOperationException($"Irreversible block retrieving strategy has been already set to {IrreversibleBlockRetrievingStrategy}");
            }

            IrreversibleBlockRetrievingStrategy = IrreversibleBlockRetrievingStrategy.Pulling;
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
            if (IrreversibleBlockRetrievingStrategy != IrreversibleBlockRetrievingStrategy.NotSupported)
            {
                throw new InvalidOperationException($"Irreversible block retrieving strategy has been already set to {IrreversibleBlockRetrievingStrategy}");
            }

            IrreversibleBlockRetrievingStrategy = IrreversibleBlockRetrievingStrategy.Pushing;
        }

        /// <summary>
        /// Disable logging in test scenarios
        /// </summary>
        internal bool DisableLogging { get; set; }
    }
}

using System;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.RabbitMq;

namespace Lykke.Blockchains.Integrations.Client.BlocksReader.Services
{
    internal class BlocksReaderApiFactory : IBlocksReaderApiFactory
    {
        private readonly IRabbitMqEndpoint _endpoint;

        public BlocksReaderApiFactory(IRabbitMqEndpoint endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        public IBlocksReaderApi Create(string integrationName)
        {
            var kebabIntegrationName = IntegrationNameTools.ToKebab(integrationName);
            var exchangeName = RabbitMqExchangeNamesFactory.GetIntegrationCommandsExchangeName(kebabIntegrationName);
            var publisher = _endpoint.CreatePublisher(exchangeName);

            return new BlocksReaderApi(publisher);
        }
    }
}

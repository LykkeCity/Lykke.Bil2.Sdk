using System;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.RabbitMq;

namespace Lykke.Bil2.Client.BlocksReader.Services
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
            var kebabIntegrationName = integrationName.CamelToKebab();
            var exchangeName = RabbitMqExchangeNamesFactory.GetIntegrationCommandsExchangeName(kebabIntegrationName);
            var publisher = _endpoint.CreatePublisher(exchangeName);

            return new BlocksReaderApi(publisher);
        }
    }
}

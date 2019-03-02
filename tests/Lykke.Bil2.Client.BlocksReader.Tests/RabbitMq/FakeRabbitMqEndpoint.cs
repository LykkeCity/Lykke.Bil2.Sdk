using System;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.Client.BlocksReader.Tests.RabbitMq
{
    public class FakeRabbitMqEndpoint : IRabbitMqEndpoint
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DeclareExchange(string exchangeName)
        {
            throw new NotImplementedException();
        }

        public IMessagePublisher CreatePublisher(string exchangeName, string correlationId = null)
        {
            throw new NotImplementedException();
        }

        public void StartListening(string listeningExchangeName, string listeningRoute,
            IMessageSubscriptionsRegistry subscriptionsRegistry, string replyExchangeName = null, int parallelism = 1)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.Client.BlocksReader.Tests.RabbitMq
{
    public class FakeRabbitMqEndpoint : IRabbitMqEndpoint
    {
        public List<FakeMessagePublisher> _publishers = new List<FakeMessagePublisher>();
        public List<FakeMessageSubscriber> _subscribers = new List<FakeMessageSubscriber>();

        public void Dispose()
        {
        }

        public void DeclareExchange(string exchangeName)
        {
            throw new NotImplementedException();
        }

        public IMessagePublisher CreatePublisher(string exchangeName, string correlationId = null)
        {
            var publisher = new FakeMessagePublisher(exchangeName, correlationId);
            _publishers.Add(publisher);

            return publisher;
        }

        public void StartListening(string listeningExchangeName, 
            string listeningRoute,
            IMessageSubscriptionsRegistry subscriptionsRegistry, 
            string replyExchangeName = null, 
            int parallelism = 1)
        {
            //var subscriber = new FakeMessageSubscriber(, (x) => x);
            //_subscribers.Add();
            //FakeMessageSubscriber();
        }

        public void Start()
        {
        }
    }
}

using Lykke.Bil2.RabbitMq.Publication;
using System;

namespace Lykke.Bil2.Client.BlocksReader.Tests.RabbitMq
{
    public delegate object PublishMessageDelegate(object @object);

    public class FakeMessagePublisher : IMessagePublisher
    {
        private readonly string _exchangeName;
        private readonly string _correlationId;

        public FakeMessagePublisher(string exchangeName, string correlationId)
        {
            _exchangeName = exchangeName;
            _correlationId = correlationId;
        }

        public event PublishMessageDelegate MessagePublishedEvent;

        public void Publish<TMessage>(TMessage message, string correlationId = null)
        {
            MessagePublishedEvent?.Invoke(message);
        }
    }

    public class FakeMessageSubscriber
    {
        private readonly Func<object, object> _processor;

        public FakeMessageSubscriber(FakeMessagePublisher publisher,
            Func<object, object> processor)
        {
            publisher.MessagePublishedEvent += ProcessMessage;
            _processor = processor;
        }

        private object ProcessMessage(object @obj)
        {
            return _processor(@obj);
        }
    }
}

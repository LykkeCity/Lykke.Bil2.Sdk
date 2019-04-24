using System;
using Lykke.Bil2.RabbitMq.MessagePack;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace Lykke.Bil2.RabbitMq.Publication
{
    internal class MessagePublisher : IMessagePublisher
    {
        private readonly string _exchangeName;
        private readonly string _correlationId;
        private readonly IModel _channel;
        private readonly ICompositeFormatterResolver _formatterResolver;

        public MessagePublisher(
            IModel channel,
            string exchangeName,
            string correlationId,
            ICompositeFormatterResolver formatterResolver)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("Should be not empty string", nameof(exchangeName));
            }

            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _exchangeName = exchangeName;
            _correlationId = correlationId;
            _formatterResolver = formatterResolver ?? throw new ArgumentNullException(nameof(formatterResolver));
        }

        public void Publish<TMessage>(TMessage message, string correlationId = null)
        {
            var properties = new BasicProperties
            {
                Persistent = true,
                CorrelationId = correlationId ?? _correlationId ?? Guid.NewGuid().ToString("N")
            };

            var serializedMessageBytes = MessagePackSerializer.Serialize(message, _formatterResolver);
            var routingKey = message.GetType().Name;

            lock (_channel)
            {
                _channel.BasicPublish(_exchangeName, routingKey, true, properties, serializedMessageBytes);
            }
        }

        public IMessagePublisher ChangeCorrelationId(string correlationId)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return new MessagePublisher(_channel, _exchangeName, correlationId, _formatterResolver);
        }
    }
}

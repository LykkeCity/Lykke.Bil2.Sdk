using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace Lykke.Bil2.RabbitMq.Publication
{
    internal class MessagePublisher : IMessagePublisher
    {
        private readonly string _exchangeName;
        private readonly string _correlationId;
        private readonly IModel _channel;

        public MessagePublisher(
            IModel channel,
            string exchangeName,
            string correlationId)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("Should be not empty string", nameof(exchangeName));
            }

            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _exchangeName = exchangeName;
            _correlationId = correlationId;
        }

        public void Publish<TMessage>(TMessage message, string correlationId = null)
        {
            var properties = new BasicProperties
            {
                Persistent = true,
                CorrelationId = _correlationId ?? correlationId ?? Guid.NewGuid().ToString("N")
            };

            var serializedMessage = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(serializedMessage);
            var routingKey = message.GetType().Name;

            lock (_channel)
            {
                _channel.BasicPublish(_exchangeName, routingKey, true, properties, messageBytes);
            }
        }
    }
}

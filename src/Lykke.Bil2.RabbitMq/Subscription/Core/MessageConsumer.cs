using System;
using System.Threading;
using Common.Log;
using Lykke.Common.Log;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal class MessageConsumer : IMessageConsumer
    {
        private static int _instanceCounter;
        
        private readonly IConnection _connection;
        private readonly IInternalMessageQueue _internalQueue;
        private readonly ILog _log;
        private readonly string _sourceQueueName;
        
        private IModel _channel;
        private readonly int _id;


        public MessageConsumer(
            IConnection connection,
            IInternalMessageQueue internalQueue,
            ILogFactory logFactory,
            string sourceQueueName)
        {
            _connection = connection;
            _internalQueue = internalQueue;
            _log = logFactory.CreateLog(this);
            _sourceQueueName = sourceQueueName;

            _id = Interlocked.Increment(ref _instanceCounter);
        }

        ~MessageConsumer()
        {
            Interlocked.Decrement(ref _instanceCounter);
        }

        public void Ack(
            ulong deliveryTag)
        {
            _channel?.BasicAck(deliveryTag, false);
        }

        public void Reject(
            ulong deliveryTag)
        {
            _channel?.BasicReject(deliveryTag, true);
        }

        public void Start()
        {
            if (_channel != null)
            {
                throw new InvalidOperationException($"Message consumer #{_id} has already been started.");
            }
            
            _channel = _connection.CreateModel();
            
            _log.Debug($"Channel #{_channel.ChannelNumber} has been created by message consumer #{_id}.");
            
            StartMessageConsumption();
            
            _log.Info($"Message consumer #{_id} has been started.");
        }

        public void Stop()
        {
            _channel?.Close();
            _channel?.Dispose();
        }

        private void ConsumerOnReceived(object sender, BasicDeliverEventArgs args)
        {
            var headers = new MessageHeaders
            (
                args.BasicProperties.CorrelationId,
                DateTimeOffset.FromUnixTimeMilliseconds(args.BasicProperties.Timestamp.UnixTime).UtcDateTime
            );
            var message = new EnvelopedMessage
            (
                body: args.Body,
                consumer: this,
                headers: headers,
                exchange: args.Exchange,
                deliveryTag: args.DeliveryTag,
                routingKey: args.RoutingKey
            );
            
            _log.Trace($"Message has been received. Channel #{_channel.ChannelNumber}. Consumer #{_id}.", message);

            _internalQueue.Enqueue(message);
        }

        private void StartMessageConsumption()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += ConsumerOnReceived;

            _channel.BasicConsume(_sourceQueueName, false, consumer);
        }
    }
}

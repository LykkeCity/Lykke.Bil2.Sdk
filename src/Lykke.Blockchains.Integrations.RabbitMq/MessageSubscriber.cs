using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.Blockchains.Integrations.RabbitMq
{
    internal class MessageSubscriber : IDisposable
    {
        private readonly ILog _log;
        private readonly IMessageSubscriptionsRegistry _subscriptionsRegistry;
        private readonly List<IModel> _channels;

        private Func<string, IMessagePublisher> _publisherFactory;

        public MessageSubscriber(
            ILogFactory logFactory,
            IConnection connection,
            string exchangeName, 
            string queueName,           
            IMessageSubscriptionsRegistry subscriptionsRegistry,
            int parallelism = 1)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("Should be not empty string", nameof(exchangeName));
            }
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Should be not empty string", nameof(queueName));
            }
            if (parallelism <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parallelism), parallelism, "Should be positive number");
            }

            _subscriptionsRegistry = subscriptionsRegistry ?? throw new ArgumentNullException(nameof(subscriptionsRegistry));

            _log = logFactory.CreateLog(this);

            using (var channel = connection.CreateModel())
            {
                channel.BasicQos(0, 100, false);
                channel.QueueDeclare(queueName, true, false, false, null);

                _log.Info($"Start receiving messages from the exchange {exchangeName} via the queue {queueName}:");

                foreach (var subscription in subscriptionsRegistry.GetAllSubscriptions())
                {
                    _log.Info($"Binding message {subscription.RoutingKey}...");

                    channel.QueueBind(queueName, exchangeName, subscription.RoutingKey, null);
                }
            }

            _channels = new List<IModel>(parallelism);

            for (var i = 0; i < parallelism; ++i)
            {
                var channel = connection.CreateModel();
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, args) =>
                {
                    HandleMessage((IModel)sender, args).ConfigureAwait(false).GetAwaiter().GetResult();
                };

                channel.BasicConsume(queueName, false, consumer);

                _channels.Add(channel);
            }
        }

        public MessageSubscriber WithRepliesPublisher(Func<string, IMessagePublisher> publisherFactory)
        {
            _publisherFactory = publisherFactory;

            return this;
        }

        public void Dispose()
        {
            foreach (var channel in _channels)
            {
                channel.Close();
                channel.Dispose();    
            }
        }

        private async Task HandleMessage(IModel channel, BasicDeliverEventArgs args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args.RoutingKey))
                {
                    _log.Error("Message without routing key is received. Skipping.");

                    channel.BasicAck(args.DeliveryTag, false);
                    return;
                }

                var subscription = _subscriptionsRegistry.GetSubscriptionOrDefault(args.RoutingKey);

                if (subscription != null)
                {
                    var messageBytes = args.Body;
                    var serializedMessage = Encoding.UTF8.GetString(messageBytes);
                    object message;

                    try
                    {
                        message = JsonConvert.DeserializeObject(serializedMessage, subscription.MessageType);
                    }
                    catch (Exception ex)
                    {
                        _log.Warning($"Failed to deserialize the message {subscription.MessageType}. Serialized message: {serializedMessage}", ex);

                        channel.BasicReject(args.DeliveryTag, true);
                        return;
                    }

                    try
                    {
                        var publisher = _publisherFactory?.Invoke(args.BasicProperties.CorrelationId) ?? 
                                        ProhibitedRepliesMessagePublisher.Instance;

                        await subscription.InvokeHandlerAsync(message, publisher);

                        channel.BasicAck(args.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _log.Warning($"Failed to process the message {subscription.MessageType} from the {args.Exchange}", ex, message);

                        channel.BasicReject(args.DeliveryTag, true);
                    }
                }
                else
                {
                    _log.Warning($"Subscription for the message the {args.RoutingKey} from the {args.Exchange} is not found");

                    channel.BasicReject(args.DeliveryTag, true);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Failed to handle the message", ex);

                channel.BasicReject(args.DeliveryTag, true);
            }
        }
    }
}

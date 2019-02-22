using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.RabbitMq.Publication;
using Lykke.Common.Log;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.Blockchains.Integrations.RabbitMq.Subscription
{
    internal class MessageSubscriber : IDisposable
    {
        private readonly ILogFactory _logFactory;
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

            _logFactory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
            _subscriptionsRegistry = subscriptionsRegistry ?? throw new ArgumentNullException(nameof(subscriptionsRegistry));

            var log = logFactory.CreateLog(this);

            using (var channel = connection.CreateModel())
            {
                channel.BasicQos(0, 100, false);
                channel.QueueDeclare(queueName, true, false, false, null);

                log.Info($"Start receiving messages from the exchange {exchangeName} via the queue {queueName}:");

                foreach (var subscription in subscriptionsRegistry.GetAllSubscriptions())
                {
                    log.Info($"Binding message {subscription.RoutingKey}...");

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
                    HandleMessage(((EventingBasicConsumer)sender).Model, args).ConfigureAwait(false).GetAwaiter().GetResult();
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
            var log = _logFactory.CreateLog(this, args.Exchange);

            try
            {
                if (string.IsNullOrWhiteSpace(args.RoutingKey))
                {
                    log.Error("Message without routing key is received. Skipping.");

                    channel.BasicAck(args.DeliveryTag, false);
                    return;
                }

                var subscription = _subscriptionsRegistry.GetSubscriptionOrDefault(args.RoutingKey);
                var messageBytes = args.Body;
                var serializedMessage = Encoding.UTF8.GetString(messageBytes);

                if (subscription != null)
                {
                    object message;

                    try
                    {
                        message = JsonConvert.DeserializeObject(serializedMessage, subscription.MessageType);
                    }
                    catch (Exception ex)
                    {
                        log.Warning(args.RoutingKey, "Failed to deserialize the message.", ex, context: serializedMessage);

                        await Task.Delay(TimeSpan.FromMinutes(10));

                        channel.BasicReject(args.DeliveryTag, true);
                        
                        return;
                    }

                    try
                    {
                        log.Trace(args.RoutingKey, "Handled", message);

                        var publisher = _publisherFactory?.Invoke(args.BasicProperties.CorrelationId) ?? 
                                        ProhibitedRepliesMessagePublisher.Instance;

                        await subscription.InvokeHandlerAsync(message, publisher);

                        channel.BasicAck(args.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        log.Warning(args.RoutingKey, "Failed to process the message", ex, message);

                        await Task.Delay(TimeSpan.FromSeconds(30));

                        channel.BasicReject(args.DeliveryTag, true);
                    }
                }
                else
                {
                    log.Warning(args.RoutingKey, "Subscription for the message is not found.", context: serializedMessage);

                    await Task.Delay(TimeSpan.FromMinutes(10));

                    channel.BasicReject(args.DeliveryTag, true);
                }
            }
            catch (Exception ex)
            {
                log.Error(args.RoutingKey, ex, "Failed to handle the message");

                await Task.Delay(TimeSpan.FromSeconds(30));

                channel.BasicReject(args.DeliveryTag, true);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Bil2.RabbitMq.MessagePack;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Common.Log;
using MoreLinq;
using RabbitMQ.Client;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal class MessageSubscriber : IDisposable
    {
        private readonly IConnection _connection;
        private readonly string _exchangeName;
        private readonly ILog _log;
        private readonly IReadOnlyCollection<IMessageConsumer> _messageConsumers;
        private readonly IReadOnlyCollection<IMessageProcessor> _messageProcessors;
        private readonly string _queueName;
        private readonly IRejectManager _rejectManager;
        private readonly IRetryManager _retryManager;
        private readonly IMessageSubscriptionsRegistry _subscriptionsRegistry;

        internal MessageSubscriber(
            IConnection connection,
            string exchangeName,
            ILogFactory logFactory,
            IReadOnlyCollection<IMessageConsumer> messageConsumers,
            IReadOnlyCollection<IMessageProcessor> messageProcessors,
            string queueName,
            IRejectManager rejectManager,
            IRetryManager retryManager,
            IMessageSubscriptionsRegistry subscriptionsRegistry)
        {
            _connection = connection;
            _exchangeName = exchangeName;
            _log = logFactory.CreateLog(this);
            _messageConsumers = messageConsumers;
            _messageProcessors = messageProcessors;
            _queueName = queueName;
            _rejectManager = rejectManager;
            _retryManager = retryManager;
            _subscriptionsRegistry = subscriptionsRegistry;
        }

        public void Dispose()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(30000);
                
                var ct = cts.Token;

                _messageConsumers.ForEach(x => x.Stop());
                
                Task.WaitAll
                (
                    _messageProcessors
                        .Select(x => x.StopAsync(ct))
                        .ToArray()
                );
                
                Task.WaitAll
                (
                    _retryManager.StopAsync(ct),
                    _rejectManager.StopAsync(ct)
                );
            }
        }

        public void StartListening()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.BasicQos(0, 100, false);
                channel.QueueDeclare(_queueName, true, false, false, null);

                _log.Info($"Start receiving messages from the exchange {_exchangeName} via the queue {_queueName}:");

                foreach (var subscription in _subscriptionsRegistry.GetAllSubscriptions())
                {
                    _log.Info($"Binding message {subscription.RoutingKey}...");

                    channel.QueueBind(_queueName, _exchangeName, subscription.RoutingKey, null);
                }
            }

            _rejectManager.Start();
            _retryManager.Start();

            _messageProcessors.ForEach(x => x.Start());
            _messageConsumers.ForEach(x => x.Start());
        }

        public static MessageSubscriber Create(
            IConnection connection,
            TimeSpan defaultRetryTimeout,
            string exchangeName,
            ICompositeFormatterResolver formatterResolver,
            int internalQueueMaxCapacity,
            ILogFactory logFactory,
            TimeSpan maxAgeForRetry,
            int maxRetryCount,
            int messageConsumersCount,
            int messageProcessorsCount,
            string queueName,
            Func<string, IMessagePublisher> repliesPublisher,
            IServiceProvider serviceProvider,
            IMessageSubscriptionsRegistry subscriptionsRegistry)
        {
            var internalQueue = new InternalMessageQueue(internalQueueMaxCapacity);
            var rejectManager = new RejectManager(logFactory);
            var retryManager = new RetryManager(logFactory, maxAgeForRetry, maxRetryCount, internalQueue);

            var messageConsumers = new List<IMessageConsumer>(messageConsumersCount);
            for (var i = 0; i < messageConsumersCount; i++)
            {
                messageConsumers.Add(new MessageConsumer
                (
                    connection,
                    internalQueue,
                    logFactory,
                    queueName
                ));
            }

            var messageProcessors = new List<IMessageProcessor>(messageConsumersCount);
            for (var i = 0; i < messageProcessorsCount; i++)
            {
                messageProcessors.Add(new MessageProcessor
                (
                    defaultRetryTimeout,
                    formatterResolver,
                    internalQueue,
                    logFactory,
                    repliesPublisher,
                    rejectManager,
                    retryManager,
                    serviceProvider,
                    subscriptionsRegistry
                ));
            }
            
            return new MessageSubscriber
            (
                connection,
                exchangeName,
                logFactory,
                messageConsumers,
                messageProcessors,
                queueName,
                rejectManager,
                retryManager,
                subscriptionsRegistry
            );
        }
    }
}

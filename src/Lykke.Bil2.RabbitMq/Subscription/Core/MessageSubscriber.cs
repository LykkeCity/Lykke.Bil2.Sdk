using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Bil2.RabbitMq.MessagePack;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Common.Log;
using MoreLinq;
using RabbitMQ.Client;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal class MessageSubscriber : IDisposable
    {
        private readonly IReadOnlyCollection<IMessageConsumer> _messageConsumers;
        private readonly IReadOnlyCollection<IMessageProcessor> _messageProcessors;
        private readonly IRejectManager _rejectManager;
        private readonly IRetryManager _retryManager;

        private MessageSubscriber(
            IReadOnlyCollection<IMessageConsumer> messageConsumers,
            IReadOnlyCollection<IMessageProcessor> messageProcessors,
            IRejectManager rejectManager,
            IRetryManager retryManager)
        {
            _messageConsumers = messageConsumers;
            _messageProcessors = messageProcessors;
            _rejectManager = rejectManager;
            _retryManager = retryManager;
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
            _rejectManager.Start();
            _retryManager.Start();

            _messageProcessors.ForEach(x => x.Start());
            _messageConsumers.ForEach(x => x.Start());
        }

        public static MessageSubscriber Create(IServiceProvider serviceProvider,
            IMessageSubscriptionsRegistry subscriptionsRegistry, 
            IConnection connection,
            ILogFactory logFactory,
            ICompositeFormatterResolver formatterResolver,
            Func<string, IMessagePublisher> repliesPublisher,
            string exchangeName,
            string queueName,
            TimeSpan defaultFirstLevelRetryTimeout,
            TimeSpan maxFirstLevelRetryMessageAge,
            int maxFirstLevelRetryCount,
            int firstLevelRetryQueueCapacity,
            int processingQueueCapacity,
            int messageConsumersCount,
            int messageProcessorsCount)
        {
            var log = logFactory.CreateLog(typeof(MessageSubscriber));

            log.Info("Creating RabbitMq subscriber...", new
            {
                exchangeName,
                queueName,
                defaultFirstLevelRetryTimeout,
                maxFirstLevelRetryMessageAge,
                maxFirstLevelRetryCount,
                firstLevelRetryQueueCapacity,
                processingQueueCapacity,
                messageConsumersCount,
                messageProcessorsCount
            });

            var internalQueue = new InternalMessageQueue(processingQueueCapacity);
            var rejectManager = new RejectManager(logFactory);
            var retryManager = new RetryManager
            (
                logFactory,
                maxFirstLevelRetryMessageAge,
                maxFirstLevelRetryCount,
                firstLevelRetryQueueCapacity,
                internalQueue
            );

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
                    defaultFirstLevelRetryTimeout,
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

            return new MessageSubscriber
            (
                messageConsumers,
                messageProcessors,
                rejectManager,
                retryManager
            );
        }
    }
}

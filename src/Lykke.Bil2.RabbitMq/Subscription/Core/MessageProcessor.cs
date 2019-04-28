using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Bil2.RabbitMq.MessagePack;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Common.Log;
using MessagePack;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal class MessageProcessor : BackgroundService, IMessageProcessor
    {
        private static int _instanceCounter;

        private readonly TimeSpan _defaultRetryTimeout;
        private readonly ICompositeFormatterResolver _formatterResolver;
        private readonly IInternalMessageQueue _internalQueue;
        private readonly ILog _log;
        private readonly ILogFactory _logFactory;
        private readonly Func<string, IMessagePublisher> _repliesPublisher;
        private readonly IRejectManager _rejectManager;
        private readonly IRetryManager _retryManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageSubscriptionsRegistry _subscriptionsRegistry;
        private readonly int _id;

        public MessageProcessor(
            TimeSpan defaultRetryTimeout,
            ICompositeFormatterResolver formatterResolver,
            IInternalMessageQueue internalQueue,
            ILogFactory logFactory,
            Func<string, IMessagePublisher> repliesPublisher,
            IRejectManager rejectManager,
            IRetryManager retryManager,
            IServiceProvider serviceProvider,
            IMessageSubscriptionsRegistry subscriptionsRegistry) : base(logFactory)
        {
            _defaultRetryTimeout = defaultRetryTimeout;
            _formatterResolver = formatterResolver;
            _internalQueue = internalQueue;
            _log = logFactory.CreateLog(this);
            _logFactory = logFactory;
            _repliesPublisher = repliesPublisher;
            _rejectManager = rejectManager;
            _retryManager = retryManager;
            _serviceProvider = serviceProvider;
            _subscriptionsRegistry = subscriptionsRegistry;
            
            _id = Interlocked.Increment(ref _instanceCounter);
        }
        
        ~MessageProcessor()
        {
            Interlocked.Decrement(ref _instanceCounter);
        }

        
        private EnvelopedMessage CurrentMessage { get; set; }
        
        private ILog CurrentLog { get; set; }


        public override void Start()
        {
            base.Start();
            
            _log.Info($"Message processor #{_id} has been started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            
            _log.Info($"Message processor #{_id} has been stopped.");
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            CurrentMessage = await _internalQueue.DequeueAsync(stoppingToken);

            if (CurrentMessage != null)
            {
                CurrentLog = _logFactory.CreateLog(this, CurrentMessage.Exchange);

                try
                {
                    if (ValidateCurrentMessage() && TryGetSubscription(out var subscription) && TryGetPayloadAndHeaders(subscription, out var payload))
                    {
                        try
                        {
                            await ProcessMessageAsync(subscription, CurrentMessage, payload);
                        }
                        catch (Exception e)
                        {
                            CurrentLog.Warning(CurrentMessage, payload, $"Failed to process the message. Processor #{_id}.", e);
                    
                            _retryManager.ScheduleRetry(CurrentMessage, retryAfter: _defaultRetryTimeout);
                        }
                    }
                }
                catch (Exception e)
                {
                    CurrentLog.Error(CurrentMessage, $"Failed to handle the message. Processor #{_id}.", e);

                    _retryManager.ScheduleRetry(CurrentMessage, retryAfter: _defaultRetryTimeout);
                }
            }
            
            CurrentMessage = null;
            CurrentLog = null;
        }

        private async Task ProcessMessageAsync(
            IMessageSubscription subscription,
            EnvelopedMessage envelopedMessage,
            object payload)
        {
            var publisher = _repliesPublisher?.Invoke(envelopedMessage.Headers.CorrelationId) 
                         ?? ProhibitedRepliesMessagePublisher.Instance;

            var handlingContext = new MessageHandlingContext
            (
                envelopedMessage.Exchange,
                envelopedMessage.RetryCount,
                envelopedMessage.RoutingKey
            );
            var result = await subscription.InvokeHandlerAsync(_serviceProvider, payload, envelopedMessage.Headers, handlingContext, publisher);

            if (result == null)
            {
                throw new InvalidOperationException("Result should be not null");
            }

            switch (result)
            {
                case MessageHandlingResult.SuccessResult _:
                case MessageHandlingResult.NonTransientFailureResult _:
                    CurrentMessage.Ack();
                    break;
                case MessageHandlingResult.TransientFailureResult tfr:
                    _retryManager.ScheduleRetry(CurrentMessage, tfr.RetryAfter ?? _defaultRetryTimeout);
                    break;
                default:
                    throw new NotSupportedException($"Unexpected message handling result [{result.GetType().Name}].");
            }
        }
        
        private bool TryGetPayloadAndHeaders(
            IMessageSubscription subscription,
            out object payload)
        {
            payload = null;
            
            try
            {
                var payloadType = subscription.MessageType;
                var payloadBytes = CurrentMessage.Body.ToArray();
                        
                payload = MessagePackSerializer.NonGeneric.Deserialize(payloadType, payloadBytes, _formatterResolver);

                return true;
            }
            catch (Exception e)
            {
                CurrentLog.Warning(CurrentMessage, $"Failed to deserialize the message. Processor #{_id}.", e);
                
                _rejectManager.ScheduleReject(CurrentMessage, rejectAfter: TimeSpan.FromMinutes(10));

                return false;
            }
        }

        private bool TryGetSubscription(
            out IMessageSubscription subscription)
        {
            subscription = _subscriptionsRegistry.GetSubscriptionOrDefault(CurrentMessage.RoutingKey);

            if (subscription != null)
            {
                return true;
            }
            else
            {
                CurrentLog.Warning(CurrentMessage, $"Subscription for the message has not been found. Processor #{_id}.");
                
                _rejectManager.ScheduleReject(CurrentMessage, rejectAfter: TimeSpan.FromMinutes(10));

                return false;
            }
        }

        private bool ValidateCurrentMessage()
        {
            if (!string.IsNullOrWhiteSpace(CurrentMessage.RoutingKey))
            {
                return true;
            }
            else
            {
                CurrentLog.Error(CurrentMessage, $"Message without routing key has been received. Skipping it. Processor #{_id}.");

                CurrentMessage.Ack();

                return false;
            }
        }
    }
}

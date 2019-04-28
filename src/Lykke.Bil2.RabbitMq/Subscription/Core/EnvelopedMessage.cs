﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal sealed class EnvelopedMessage
    {
        private readonly IMessageConsumer _consumer;
        private readonly DateTime _createdOn;
        private readonly ulong _deliveryTag;

        public EnvelopedMessage(
            IEnumerable<byte> body,
            IMessageConsumer consumer,
            MessageHeaders headers,
            string exchange,
            ulong deliveryTag,
            string routingKey)
        
            : this(body, consumer, DateTime.UtcNow, headers, deliveryTag, exchange, Guid.NewGuid(),0, routingKey)
        {
            
        }
        
        private EnvelopedMessage(
            IEnumerable<byte> body,
            IMessageConsumer consumer,
            DateTime createdOn,
            MessageHeaders headers,
            ulong deliveryTag,
            string exchange,
            Guid id,
            int retryCount,
            string routingKey)
        {
            _consumer = consumer;
            _createdOn = createdOn;
            _deliveryTag = deliveryTag;

            Body = body.ToImmutableArray();
            Headers = headers;
            Exchange = exchange;
            Id = id;
            RetryCount = retryCount;
            RoutingKey = routingKey;
        }


        public TimeSpan Age
            => DateTime.UtcNow - _createdOn;
        
        public ImmutableArray<byte> Body { get; }
        
        public MessageHeaders Headers { get; }
        
        public string Exchange { get; }
        
        [UsedImplicitly]
        public Guid Id { get; }
        
        public int RetryCount { get; }
        
        public string RoutingKey { get; }
        
        
        public void Ack()
        {
            _consumer.Ack(_deliveryTag);
        }

        public void Reject()
        {
            _consumer.Reject(_deliveryTag);
        }

        public EnvelopedMessage WithIncreasedRetryCount()
        {
            var newRetryCount = RetryCount + 1;
            
            return new EnvelopedMessage
            (
                Body,
                _consumer,
                _createdOn,
                Headers,
                _deliveryTag,
                Exchange,
                Id,
                newRetryCount,
                RoutingKey
            );
        }
    }
}

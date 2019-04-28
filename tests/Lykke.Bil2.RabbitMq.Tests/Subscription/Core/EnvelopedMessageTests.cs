using System;
using System.Linq;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.RabbitMq.Subscription.Core;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Core
{
    [TestFixture]
    public class EnvelopedMessageTests
    {
        [Test]
        public void Test_that_Ack_call_calls_Ack_method_of_underlying_consumer_with_correct_delivery_tag()
        {
            const ulong deliveryTag = 42;
            
            var consumerMock = new Mock<IMessageConsumer>();
            
            var message = new EnvelopedMessage
            (
                Enumerable.Empty<byte>(),
                consumerMock.Object,
                new MessageHeaders(string.Empty, DateTime.UtcNow), 
                string.Empty,
                deliveryTag,
                string.Empty
            );
            
            message.Ack();
            
            consumerMock.Verify(x => x.Ack(deliveryTag), Times.Once);
        }
        
        [Test]
        public void Test_that_Reject_call_calls_Reject_method_of_underlying_consumer_with_correct_delivery_tag()
        {
            const ulong deliveryTag = 42;
            
            var consumerMock = new Mock<IMessageConsumer>();
            
            var message = new EnvelopedMessage
            (
                Enumerable.Empty<byte>(),
                consumerMock.Object,
                new MessageHeaders(string.Empty, DateTime.UtcNow), 
                string.Empty,
                deliveryTag,
                string.Empty
            );
            
            message.Reject();
            
            consumerMock.Verify(x => x.Reject(deliveryTag), Times.Once);
        }

        [Test]
        public void Test_that_WithIncreasedRetryCount_creates_new_message_instance_with_incremented_RetryCount_value()
        {
            var consumerMock = new Mock<IMessageConsumer>();
            
            var right = new EnvelopedMessage
            (
                Enumerable.Empty<byte>(),
                consumerMock.Object,
                new MessageHeaders(string.Empty, DateTime.UtcNow), 
                string.Empty,
                42,
                string.Empty
            );

            for (var i = 0; i < 10; i++)
            {
                var left = right.WithIncreasedRetryCount();
                
                Assert.AreNotEqual(left, right);
                Assert.AreEqual(1, left.RetryCount - right.RetryCount);

                right = left;
            }
        }
    }
}

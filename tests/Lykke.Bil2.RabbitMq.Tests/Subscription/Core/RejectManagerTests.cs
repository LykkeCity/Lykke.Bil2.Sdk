using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.RabbitMq.Subscription.Core;
using Lykke.Logs;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Core
{
    [TestFixture]
    public class RejectManagerTests
    {
        [Test]
        public async Task Test_that_messages_are_rejected_in_right_order()
        {
            var consumerMock = new Mock<IMessageConsumer>();
            var deliveryTagsInActualOrder = new List<ulong>();
            
            consumerMock
                .Setup(x => x.Reject(It.IsAny<ulong>()))
                .Callback<ulong>(tag => { deliveryTagsInActualOrder.Add(tag); });


            var utcNowProvider = new UtcNowProvider
            {
                NextValue = DateTime.UtcNow
            };
            
            var rejectManager = new RejectManager
            (
                EmptyLogFactory.Instance,
                () => utcNowProvider.NextValue
            );

            var deliveryTagsInExpectedOrder = Enumerable
                .Range(0, 999)
                .OrderBy(x => Guid.NewGuid())
                .Select(x => (ulong) x)
                .ToList();

            var rejectAfter = 0;
            
            foreach (var deliveryTag in deliveryTagsInExpectedOrder)
            {
                rejectManager.ScheduleReject
                (
                    GenerateMessage(deliveryTag, consumerMock.Object),
                    new TimeSpan(rejectAfter++)
                );
            }
            
            utcNowProvider.NextValue = utcNowProvider.NextValue.AddMilliseconds(rejectAfter);
            
            rejectManager.Start();
            
            await rejectManager.StopAsync();

            
            deliveryTagsInActualOrder
                .Should()
                .Equal(deliveryTagsInExpectedOrder);
        }

        private static EnvelopedMessage GenerateMessage(
            ulong deliveryTag,
            IMessageConsumer messageConsumer)
        {
            return new EnvelopedMessage
            (
                Enumerable.Empty<byte>(),
                messageConsumer,
                new MessageHeaders(string.Empty, DateTime.UtcNow), 
                string.Empty,
                deliveryTag,
                string.Empty
            );
        }

        private class UtcNowProvider
        {
            public DateTime NextValue { get; set; }
            
            public DateTime GetUtcNow()
            {
                return NextValue;
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks
{
    internal interface ITestMessageHandlerImplementation : IDisposable
    {
        Task<MessageHandlingResult> HandleAsync(TestMessage message, MessageHeaders headers, IMessagePublisher replyPublisher);
    }
}

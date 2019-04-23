using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks
{
    [UsedImplicitly]
    internal class TestMessageHandler : IMessageHandler<TestMessage>
    {
        private readonly ITestMessageHandlerImplementation _impl;

        public TestMessageHandler(ITestMessageHandlerImplementation impl)
        {
            _impl = impl;
        }

        public Task<MessageHandlingResult> HandleAsync(TestMessage message, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return _impl.HandleAsync(message, headers, replyPublisher);
        }
    }
}

using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks
{
    [UsedImplicitly]
    internal class TestMessageHandlerWithState : IMessageHandler<TestMessage, string>
    {
        private readonly ITestMessageHandlerWithStateImplementation _impl;

        public TestMessageHandlerWithState(ITestMessageHandlerWithStateImplementation impl)
        {
            _impl = impl;
        }

        public Task<MessageHandlingResult> HandleAsync(string state, TestMessage message, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return _impl.HandleAsync(state, message, headers, replyPublisher);
        }
    }
}
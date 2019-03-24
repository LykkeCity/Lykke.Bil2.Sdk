using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks
{
    [UsedImplicitly]
    internal class TestMessageHandlerWithState : IMessageHandler<TestMessage, string>
    {
        private readonly DisposableDependency _dependency;

        public TestMessageHandlerWithState(DisposableDependency dependency)
        {
            _dependency = dependency;
        }

        public Task HandleAsync(string state, TestMessage message, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return _dependency.FooWithStateAsync(message.Id, state);
        }
    }
}

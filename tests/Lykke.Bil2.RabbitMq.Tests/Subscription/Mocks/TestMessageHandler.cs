using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks
{
    [UsedImplicitly]
    internal class TestMessageHandler : IMessageHandler<TestMessage>
    {
        private readonly DisposableDependency _dependency;

        public TestMessageHandler(DisposableDependency dependency)
        {
            _dependency = dependency;
        }

        public Task HandleAsync(TestMessage message, IMessagePublisher publisher)
        {
            return _dependency.FooAsync(message.Id);
        }
    }
}

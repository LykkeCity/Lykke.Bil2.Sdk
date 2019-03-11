using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks
{
    [UsedImplicitly]
    internal class DisposableDependency : IDisposable
    {
        private readonly ISet<string> _calls;
        private readonly ManualResetEventSlim _disposalEvent;

        public DisposableDependency(ISet<string> calls, ManualResetEventSlim disposalEvent)
        {
            _calls = calls;
            _disposalEvent = disposalEvent;
        }

        public Task FooAsync(Guid id)
        {
            _calls.Add(nameof(FooAsync));

            return Task.CompletedTask;
        }

        public Task FooWithStateAsync(Guid id, string state)
        {
            _calls.Add(nameof(FooWithStateAsync));

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _disposalEvent.Set();
        }
    }
}

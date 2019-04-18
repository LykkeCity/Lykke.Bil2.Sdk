using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Subscription.Core;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Core
{
    [TestFixture]
    public class BackgroundServiceTests
    {
        [UsedImplicitly]
        internal class BackgroundServiceImpl : BackgroundService
        {
            public BackgroundServiceImpl() : 
                base(LogFactory.Create().AddUnbufferedConsole())
            {
            }

            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                return Task.CompletedTask;
            }
        }

        [Test]
        public async Task Test_that_parameterless_StopAsync_call_uses_an_empty_cancellation_token()
        {
            var mock = new Mock<BackgroundServiceImpl> {CallBase = true};

            await mock.Object.StopAsync();
            
            mock.Verify(x => x.StopAsync(CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task Test_that_StopAsync_call_prior_to_StartAsync_call_does_not_throw_an_exception()
        {
            var mock = new Mock<BackgroundServiceImpl> {CallBase = true};

            try
            {
                await mock.Object.StopAsync();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task Test_that_stopping_token_is_cancelled_on_StopAsync_call()
        {
            var probe = new BackgroundServiceProbe();

            probe.Start();
            
            var stopTask = probe.StopAsync();

            probe.CompleteExecution();

            await stopTask;
            
            Assert.IsTrue(probe.StoppingToken.IsCancellationRequested);
        }

        [Test]
        public async Task Test_that_StopAsync_call_waits_for_underlying_ExecutingAsync_completion()
        {
            var probe = new BackgroundServiceProbe();

            probe.Start();
            
            var stopTask = probe.StopAsync();

            await Task.Delay(100);
            
            Assert.IsFalse(probe.ExecutionCompleted);
            
            probe.CompleteExecution();

            await stopTask;
            
            Assert.IsTrue(probe.ExecutionCompleted);
        }
        
        private class BackgroundServiceProbe : BackgroundService
        {
            private readonly TaskCompletionSource<bool> _executeAsyncCompletionSource;
            
            public BackgroundServiceProbe() : base(LogFactory.Create().AddUnbufferedConsole())
            {
                _executeAsyncCompletionSource = new TaskCompletionSource<bool>();
            }


            public bool ExecutionCompleted { get; private set; }
            
            public CancellationToken StoppingToken { get; private set; }


            public void CompleteExecution()
            {
                _executeAsyncCompletionSource.SetResult(true);
            }
            
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                StoppingToken = stoppingToken;

                await _executeAsyncCompletionSource.Task;

                ExecutionCompleted = true;
            }
        }
    }
}

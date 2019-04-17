using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal abstract class BackgroundService : IBackgroundService
    {
        private readonly CancellationTokenSource _stoppingCts;
        
        private Task _executingTask;

        
        protected BackgroundService()
        {
            _stoppingCts = new CancellationTokenSource();
        }

        protected abstract Task ExecuteAsync(
            CancellationToken stoppingToken);

        public virtual void Start()
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);
        }

        public virtual Task StopAsync()
            => StopAsync(CancellationToken.None);

        public virtual async Task StopAsync(
            CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }
            
            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await SilentlyWaitAsync(_executingTask, cancellationToken);
            }
        }

        protected static async Task SilentlyDelayAsync(
            int millisecondsDelay,
            CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                
            }
        }

        private static async Task SilentlyWaitAsync(
            Task executingTask,
            CancellationToken cancellationToken)
        {
            try
            {
                await executingTask.WaitAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                
            }
        }
    }
}

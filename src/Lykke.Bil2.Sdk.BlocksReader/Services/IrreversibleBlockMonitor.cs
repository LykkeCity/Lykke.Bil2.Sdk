using System;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class IrreversibleBlockMonitor : 
        IIrreversibleBlockMonitor,
        IDisposable
    {
        private readonly IIrreversibleBlockProvider _irreversibleBlockProvider;
        private readonly IIrreversibleBlockListener _irreversibleBlockListener;
        private readonly TimerTrigger _timer;

        public IrreversibleBlockMonitor(
            ILogFactory logFactory,
            IIrreversibleBlockProvider irreversibleBlockProvider,
            IIrreversibleBlockListener irreversibleBlockListener,
            TimeSpan monitoringPeriod)
        {
            _irreversibleBlockProvider = irreversibleBlockProvider;
            _irreversibleBlockListener = irreversibleBlockListener;

            if (monitoringPeriod <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(monitoringPeriod), monitoringPeriod, "Should be positive time period");
            }

            _timer = new TimerTrigger(nameof(IrreversibleBlockMonitor), monitoringPeriod, logFactory);
            _timer.Triggered += HandleMonitoringTimer;
        }

        private async Task HandleMonitoringTimer(ITimerTrigger timer, TimerTriggeredHandlerArgs args, System.Threading.CancellationToken cancellationToken)
        {
            var evt = await _irreversibleBlockProvider.GetLastAsync();

            await _irreversibleBlockListener.HandleNewLastIrreversableBlockAsync(evt);
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}

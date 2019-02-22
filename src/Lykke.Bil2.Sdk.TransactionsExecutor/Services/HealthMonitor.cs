using System;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    internal class HealthMonitor : 
        IHealthMonitor,
        IDisposable
    {
        public string Disease { get; private set; }

        private readonly IHealthProvider _healthProvider;
        private readonly TimerTrigger _timer;

        public HealthMonitor(
            ILogFactory logFactory,
            IHealthProvider healthProvider,
            TimeSpan monitoringPeriod)
        {
            _healthProvider = healthProvider;
            _timer = new TimerTrigger(nameof(HealthMonitor), monitoringPeriod, logFactory);
            _timer.Triggered += HandleMonitoringTimer;
        }

        private async Task HandleMonitoringTimer(ITimerTrigger timer, TimerTriggeredHandlerArgs args, System.Threading.CancellationToken cancellationToken)
        {
            Disease = await _healthProvider.GetDiseaseAsync();
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

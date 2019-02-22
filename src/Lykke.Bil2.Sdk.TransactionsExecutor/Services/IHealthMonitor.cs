using Autofac;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    internal interface IHealthMonitor : IStartable
    {
        string Disease { get; }
    }
}

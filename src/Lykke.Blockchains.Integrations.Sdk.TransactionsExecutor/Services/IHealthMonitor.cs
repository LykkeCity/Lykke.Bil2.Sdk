using Autofac;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services
{
    internal interface IHealthMonitor : IStartable
    {
        string Disease { get; }
    }
}

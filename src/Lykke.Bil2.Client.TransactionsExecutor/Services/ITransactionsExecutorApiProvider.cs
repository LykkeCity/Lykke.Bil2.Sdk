using Lykke.Bil2.WebClient.Services;

namespace Lykke.Bil2.Client.TransactionsExecutor.Services
{
    /// <summary>
    /// Provider of the integration transactions executor web client API.
    /// </summary>
    public interface ITransactionsExecutorApiProvider : IBaseWebClientApiProvider<ITransactionsExecutorApi>
    {
    }
}

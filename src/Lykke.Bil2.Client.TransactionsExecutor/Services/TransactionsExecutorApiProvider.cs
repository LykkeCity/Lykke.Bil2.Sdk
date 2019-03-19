using System.Collections.Generic;
using Lykke.Bil2.WebClient.Services;

namespace Lykke.Bil2.Client.TransactionsExecutor.Services
{
    internal class TransactionsExecutorApiProvider : 
        BaseWebClientApiProvider<ITransactionsExecutorApi>,
        ITransactionsExecutorApiProvider
    {
        public TransactionsExecutorApiProvider(IReadOnlyDictionary<string, ITransactionsExecutorApi> integrationApis) : 
            base(integrationApis)
        {
        }
    }
}

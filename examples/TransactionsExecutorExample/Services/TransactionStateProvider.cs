﻿using System.Threading.Tasks;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;

namespace TransactionsExecutorExample.Services
{
    public class TransactionStateProvider : ITransactionsStateProvider
    {
        public Task<TransactionState> GetStateAsync(string transactionId)
        {
            return Task.FromResult(TransactionState.Broadcasted);
        }
    }
}
